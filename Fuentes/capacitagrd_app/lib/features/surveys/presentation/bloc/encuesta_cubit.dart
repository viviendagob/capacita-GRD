import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../data/models/encuesta_model.dart';
import '../../domain/repositories/encuesta_repository.dart';

part 'encuesta_state.dart';

class EncuestaCubit extends Cubit<EncuestaState> {
  final EncuestaRepository repository;
  final Map<int, String> _respuestasDraft = {};

  EncuestaCubit({required this.repository}) : super(EncuestaInitial());

  Future<void> cargarEncuesta(int idEncuesta) async {
    emit(EncuestaLoading());
    final result = await repository.getEncuesta(idEncuesta);
    result.fold(
      (failure) => emit(EncuestaError(failure.message)),
      (encuesta) => emit(EncuestaLoaded(encuesta, {})),
    );
  }

  void responder(int idPregunta, String respuesta) {
    _respuestasDraft[idPregunta] = respuesta;
    if (state is EncuestaLoaded) {
      emit(EncuestaLoaded((state as EncuestaLoaded).encuesta, Map.from(_respuestasDraft)));
    }
  }

  Future<void> guardarRespuestas({
    required int idPersona,
    required int idEvento,
  }) async {
    final loaded = state as EncuestaLoaded?;
    if (loaded == null) return;

    final respuestas = _respuestasDraft.entries.map((e) => RespuestaParticipanteModel(
          idEncuesta: loaded.encuesta.idEncuesta,
          idPersona: idPersona,
          idEvento: idEvento,
          idPregunta: e.key,
          respuesta: e.value,
          fechaRespuesta: DateTime.now(),
        )).toList();

    final result = await repository.guardarRespuestasLocal(respuestas);
    result.fold(
      (failure) => emit(EncuestaError(failure.message)),
      (_) => emit(EncuestaCompletada()),
    );

    // El guardado local ya completó la encuesta para el usuario; el envío al
    // servidor se intenta en segundo plano y no bloquea el flujo si no hay red.
    await repository.enviarRespuestas(respuestas);
  }
}
