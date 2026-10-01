part of 'persona_cubit.dart';

abstract class PersonaState extends Equatable {
  const PersonaState();
  @override
  List<Object?> get props => [];
}

class PersonaInitial extends PersonaState {}
class PersonaLoading extends PersonaState {}
class PersonaActualizando extends PersonaState {}
class PersonaCreando extends PersonaState {}
class PersonaActualizadaExito extends PersonaState {}

class PersonaLoaded extends PersonaState {
  final PersonaModel persona;
  final PersonaDataModel? personaData;
  const PersonaLoaded(this.persona, {this.personaData});
  @override
  List<Object?> get props => [persona, personaData];
}

class PersonaCreadaExito extends PersonaState {
  final PersonaModel persona;
  const PersonaCreadaExito(this.persona);
  @override
  List<Object?> get props => [persona];
}

class PersonaError extends PersonaState {
  final String message;
  const PersonaError(this.message);
  @override
  List<Object?> get props => [message];
}
