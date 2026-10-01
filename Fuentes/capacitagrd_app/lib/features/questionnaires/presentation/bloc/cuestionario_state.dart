part of 'cuestionario_cubit.dart';

abstract class CuestionarioState extends Equatable {
  const CuestionarioState();
  @override
  List<Object?> get props => [];
}

class CuestionarioInitial extends CuestionarioState {}
class CuestionarioLoading extends CuestionarioState {}
class CuestionarioEnviando extends CuestionarioState {}

class CuestionarioLoaded extends CuestionarioState {
  final CuestionarioModel cuestionario;
  final Map<int, int> respuestas;
  const CuestionarioLoaded(this.cuestionario, this.respuestas);

  bool get completo => cuestionario.preguntas.every((p) =>
      respuestas.containsKey(p.idCuestionarioPregunta));

  @override
  List<Object?> get props => [cuestionario, respuestas];
}

class CuestionarioCompletado extends CuestionarioState {
  final ResultadoCuestionario resultado;
  const CuestionarioCompletado(this.resultado);
  @override
  List<Object?> get props => [resultado];
}

class CuestionarioError extends CuestionarioState {
  final String message;
  const CuestionarioError(this.message);
  @override
  List<Object?> get props => [message];
}
