import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../data/models/evento_model.dart';
import '../../domain/repositories/eventos_repository.dart';

part 'eventos_state.dart';

class EventosCubit extends Cubit<EventosState> {
  final EventosRepository repository;

  EventosCubit({required this.repository}) : super(EventosInitial());

  Future<void> loadMisEventos(int idPersona) async {
    emit(EventosLoading());
    final result = await repository.getMisEventos(idPersona);
    result.fold(
      (failure) => emit(EventosError(failure.message)),
      (eventos) => emit(EventosLoaded(eventos)),
    );
  }

  Future<void> loadEventosDisponibles() async {
    emit(EventosLoading());
    final result = await repository.getEventosDisponibles();
    result.fold(
      (failure) => emit(EventosError(failure.message)),
      (eventos) => emit(EventosDisponiblesLoaded(eventos)),
    );
  }

  Future<void> loadEvento(int idEvento) async {
    emit(EventoDetailLoading());
    final result = await repository.getEvento(idEvento);
    result.fold(
      (failure) => emit(EventosError(failure.message)),
      (evento) => emit(EventoDetailLoaded(evento)),
    );
  }

  Future<void> inscribirse({
    required int idEvento,
    required int idPersona,
    required int idPersonaData,
    required int idModalidad,
    double? latitud,
    double? longitud,
  }) async {
    emit(EventoInscribiendo());
    final result = await repository.inscribirseEvento(
      idEvento: idEvento,
      idPersona: idPersona,
      idPersonaData: idPersonaData,
      idModalidad: idModalidad,
      latitud: latitud,
      longitud: longitud,
    );
    result.fold(
      (failure) => emit(EventosError(failure.message)),
      (participante) => emit(EventoInscritoExito(participante)),
    );
  }
}
