import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/persona_model.dart';
import '../../data/models/persona_data_model.dart';

abstract class PersonaRepository {
  Future<Either<Failure, PersonaModel>> getPersona(int idPersona);
  Future<Either<Failure, PersonaModel?>> buscarPorDocumento(String numDocumento);
  Future<Either<Failure, bool>> existePersona(String numDocumento);
  Future<Either<Failure, PersonaModel>> crearPersona(PersonaModel persona, PersonaDataModel personaData);
  Future<Either<Failure, void>> actualizarPersona(PersonaModel persona);
  Future<Either<Failure, PersonaDataModel>> getPersonaData(int idPersonaData);
  Future<Either<Failure, void>> actualizarPersonaData(PersonaDataModel personaData);
}
