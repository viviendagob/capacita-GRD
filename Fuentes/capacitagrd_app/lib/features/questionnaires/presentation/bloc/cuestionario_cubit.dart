import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../data/models/cuestionario_model.dart';
import '../../domain/repositories/cuestionario_repository.dart';

part 'cuestionario_state.dart';

class CuestionarioCubit extends Cubit<CuestionarioState> {
  final CuestionarioRepository repository;
  final Map<int, int> _respuestasDraft = {}; // idPregunta -> idRespuestaSeleccionada

  CuestionarioCubit({required this.repository}) : super(CuestionarioInitial());

  Future<void> cargarCuestionario(int idCuestionario) async {
    emit(CuestionarioLoading());
    final result = await repository.getCuestionario(idCuestionario);
    result.fold(
      (failure) => emit(CuestionarioError(failure.message)),
      (cuestionario) => emit(CuestionarioLoaded(cuestionario, {})),
    );
  }

  void seleccionarRespuesta(int idPregunta, int idRespuesta) {
    _respuestasDraft[idPregunta] = idRespuesta;
    if (state is CuestionarioLoaded) {
      emit(CuestionarioLoaded((state as CuestionarioLoaded).cuestionario, Map.from(_respuestasDraft)));
    }
  }

  Future<void> enviarCuestionario({
    required int idPersona,
    required int idEvento,
  }) async {
    final loaded = state as CuestionarioLoaded?;
    if (loaded == null) return;

    emit(CuestionarioEnviando());

    final cuestionario = loaded.cuestionario;
    int puntajeObtenido = 0;

    final respuestas = _respuestasDraft.entries.map((entry) {
      final pregunta = cuestionario.preguntas.firstWhere(
        (p) => p.idCuestionarioPregunta == entry.key,
        orElse: () => const CuestionarioPreguntaModel(
            idCuestionarioPregunta: 0, nombre: '', peso: 0, idCuestionario: 0),
      );
      // Find if selected answer is "correct" — API doesn't expose correct answer
      // Score all answers with full peso (scoring logic TBD when API supports it)
      puntajeObtenido += pregunta.peso;

      return RespuestaCuestionarioModel(
        idCuestionario: cuestionario.idCuestionario,
        idPersona: idPersona,
        idEvento: idEvento,
        idPregunta: entry.key,
        idRespuestaSeleccionada: entry.value,
        fechaRespuesta: DateTime.now(),
      );
    }).toList();

    final result = await repository.guardarRespuestasLocal(respuestas);
    result.fold(
      (failure) => emit(CuestionarioError(failure.message)),
      (_) => emit(CuestionarioCompletado(ResultadoCuestionario(
        puntajeObtenido: puntajeObtenido,
        puntajeTotal: cuestionario.puntajeTotal,
        aprobado: puntajeObtenido >= cuestionario.puntajeTotal * 0.6,
        respuestas: respuestas,
      ))),
    );
  }
}
