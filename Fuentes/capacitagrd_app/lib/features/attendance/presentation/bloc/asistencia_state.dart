part of 'asistencia_cubit.dart';

abstract class AsistenciaState extends Equatable {
  const AsistenciaState();
  @override
  List<Object?> get props => [];
}

class AsistenciaInitial extends AsistenciaState {}
class AsistenciaLoading extends AsistenciaState {}
class AsistenciaRegistrando extends AsistenciaState {}

class AsistenciasLoaded extends AsistenciaState {
  final List<AsistenciaModel> asistencias;
  const AsistenciasLoaded(this.asistencias);
  @override
  List<Object?> get props => [asistencias];
}

class AsistenciaRegistradaExito extends AsistenciaState {
  final AsistenciaModel asistencia;
  const AsistenciaRegistradaExito(this.asistencia);
  @override
  List<Object?> get props => [asistencia];
}

class AsistenciaError extends AsistenciaState {
  final String message;
  const AsistenciaError(this.message);
  @override
  List<Object?> get props => [message];
}
