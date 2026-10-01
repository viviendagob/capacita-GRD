import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../data/models/asistencia_model.dart';
import '../../domain/repositories/asistencia_repository.dart';
import '../../../events/domain/repositories/eventos_repository.dart';

part 'asistencia_state.dart';

// Formato del QR personal que genera la API al inscribirse (ver
// EventosParticipantesEndpoints.GenerarQR): "CAPACITA-GRD:EVENTO:{idEvento}:PERSONA:{idPersona}".
final _qrPattern = RegExp(r'^CAPACITA-GRD:EVENTO:(\d+):PERSONA:(\d+)$');

class AsistenciaCubit extends Cubit<AsistenciaState> {
  final AsistenciaRepository repository;
  final EventosRepository eventosRepository;

  AsistenciaCubit({required this.repository, required this.eventosRepository}) : super(AsistenciaInitial());

  Future<void> cargarAsistencias(int idEvento, int idPersona) async {
    emit(AsistenciaLoading());
    final result = await repository.getAsistencias(idEvento, idPersona);
    result.fold(
      (failure) => emit(AsistenciaError(failure.message)),
      (list) => emit(AsistenciasLoaded(list)),
    );
  }

  // Lo usa el staff (rol ADMIN) para validar la asistencia de un participante escaneando SU
  // QR personal — no es el propio participante escaneando el suyo.
  Future<void> validarAsistenciaPorQR({
    required int idEvento,
    required String qrData,
  }) async {
    emit(AsistenciaRegistrando());

    final match = _qrPattern.firstMatch(qrData.trim());
    if (match == null) {
      emit(const AsistenciaError('Este QR no es válido para registrar asistencia.'));
      return;
    }

    final idEventoQr = int.parse(match.group(1)!);
    final idPersonaQr = int.parse(match.group(2)!);
    if (idEventoQr != idEvento) {
      emit(const AsistenciaError('Este QR pertenece a otro evento.'));
      return;
    }

    final inscripcionResult = await eventosRepository.verificarInscripcion(idEvento, idPersonaQr);
    final participante = inscripcionResult.fold((_) => null, (v) => v);
    if (participante == null) {
      emit(const AsistenciaError('Esta persona no está inscrita en este evento.'));
      return;
    }

    final request = RegistrarAsistenciaRequest(
      idEvento: idEvento,
      idPersona: idPersonaQr,
      idModalidad: participante.idModalidad,
      fecha: DateTime.now(),
    );

    final result = await repository.registrarAsistencia(request);
    result.fold(
      (failure) => emit(AsistenciaError(failure.message)),
      (asistencia) => emit(AsistenciaRegistradaExito(asistencia)),
    );
  }
}
