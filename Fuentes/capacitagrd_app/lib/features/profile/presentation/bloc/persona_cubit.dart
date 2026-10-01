import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../../data/models/persona_model.dart';
import '../../data/models/persona_data_model.dart';
import '../../domain/repositories/persona_repository.dart';

part 'persona_state.dart';

class PersonaCubit extends Cubit<PersonaState> {
  final PersonaRepository repository;
  final FlutterSecureStorage secureStorage;

  PersonaCubit({required this.repository, required this.secureStorage}) : super(PersonaInitial());

  Future<void> cargarPerfil() async {
    emit(PersonaLoading());
    final idStr = await secureStorage.read(key: AppConstants.personaIdKey);
    if (idStr == null) {
      emit(const PersonaError('Sesión inválida'));
      return;
    }
    final idPersona = int.tryParse(idStr) ?? 0;
    final result = await repository.getPersona(idPersona);
    result.fold(
      (failure) => emit(PersonaError(failure.message)),
      (persona) => emit(PersonaLoaded(persona)),
    );
  }

  Future<void> cargarPersonaData() async {
    final idStr = await secureStorage.read(key: AppConstants.personaDataIdKey);
    if (idStr == null) return;
    final idPersonaData = int.tryParse(idStr) ?? 0;
    final result = await repository.getPersonaData(idPersonaData);
    result.fold(
      (failure) {},
      (personaData) {
        if (state is PersonaLoaded) {
          emit(PersonaLoaded((state as PersonaLoaded).persona, personaData: personaData));
        }
      },
    );
  }

  Future<void> actualizarPersona(PersonaModel persona) async {
    emit(PersonaActualizando());
    final result = await repository.actualizarPersona(persona);
    result.fold(
      (failure) => emit(PersonaError(failure.message)),
      (_) => emit(PersonaActualizadaExito()),
    );
  }

  Future<void> registrarNuevoParticipante(PersonaModel persona, PersonaDataModel personaData) async {
    emit(PersonaCreando());
    final result = await repository.crearPersona(persona, personaData);
    result.fold(
      (failure) => emit(PersonaError(failure.message)),
      (nuevaPersona) async {
        await secureStorage.write(key: AppConstants.personaIdKey, value: nuevaPersona.idPersona.toString());
        emit(PersonaCreadaExito(nuevaPersona));
      },
    );
  }
}
