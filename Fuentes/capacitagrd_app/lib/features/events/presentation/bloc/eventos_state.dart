part of 'eventos_cubit.dart';

abstract class EventosState extends Equatable {
  const EventosState();
  @override
  List<Object?> get props => [];
}

class EventosInitial extends EventosState {}
class EventosLoading extends EventosState {}
class EventoDetailLoading extends EventosState {}
class EventoInscribiendo extends EventosState {}

class EventoInscritoExito extends EventosState {
  final EventoParticipanteModel participante;
  const EventoInscritoExito(this.participante);
  @override
  List<Object?> get props => [participante];
}

class EventosLoaded extends EventosState {
  final List<EventoParticipanteModel> eventos;
  const EventosLoaded(this.eventos);
  @override
  List<Object?> get props => [eventos];
}

class EventosDisponiblesLoaded extends EventosState {
  final List<EventoModel> eventos;
  const EventosDisponiblesLoaded(this.eventos);
  @override
  List<Object?> get props => [eventos];
}

class EventoDetailLoaded extends EventosState {
  final EventoModel evento;
  const EventoDetailLoaded(this.evento);
  @override
  List<Object?> get props => [evento];
}

class EventosError extends EventosState {
  final String message;
  const EventosError(this.message);
  @override
  List<Object?> get props => [message];
}
