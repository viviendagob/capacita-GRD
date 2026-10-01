part of 'encuesta_cubit.dart';

abstract class EncuestaState extends Equatable {
  const EncuestaState();
  @override
  List<Object?> get props => [];
}

class EncuestaInitial extends EncuestaState {}
class EncuestaLoading extends EncuestaState {}
class EncuestaCompletada extends EncuestaState {}

class EncuestaLoaded extends EncuestaState {
  final EncuestaModel encuesta;
  final Map<int, String> respuestas;
  const EncuestaLoaded(this.encuesta, this.respuestas);

  bool get completa => encuesta.preguntas.every((p) =>
      respuestas.containsKey(p.idRespuesta) && respuestas[p.idRespuesta]!.isNotEmpty);

  @override
  List<Object?> get props => [encuesta, respuestas];
}

class EncuestaError extends EncuestaState {
  final String message;
  const EncuestaError(this.message);
  @override
  List<Object?> get props => [message];
}
