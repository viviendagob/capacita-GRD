import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/persona_repository.dart';
import '../datasources/persona_remote_datasource.dart';
import '../models/persona_model.dart';
import '../models/persona_data_model.dart';

class PersonaRepositoryImpl implements PersonaRepository {
  final PersonaRemoteDataSource remote;
  const PersonaRepositoryImpl({required this.remote});

  @override
  Future<Either<Failure, PersonaModel>> getPersona(int idPersona) async {
    try {
      return Right(await remote.getPersona(idPersona));
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, PersonaModel?>> buscarPorDocumento(String numDocumento) async {
    try {
      return Right(await remote.buscarPorDocumento(numDocumento));
    } on DioException catch (e) {
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, bool>> existePersona(String numDocumento) async {
    try {
      return Right(await remote.existePersona(numDocumento));
    } on DioException catch (e) {
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, PersonaModel>> crearPersona(PersonaModel persona, PersonaDataModel personaData) async {
    try {
      return Right(await remote.crearPersona(persona, personaData));
    } on DioException catch (e) {
      if (e.response?.statusCode == 409) return Left(ValidationFailure('El documento ya está registrado'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> actualizarPersona(PersonaModel persona) async {
    try {
      await remote.actualizarPersona(persona);
      return const Right(null);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, PersonaDataModel>> getPersonaData(int idPersonaData) async {
    try {
      return Right(await remote.getPersonaData(idPersonaData));
    } on DioException catch (e) {
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> actualizarPersonaData(PersonaDataModel personaData) async {
    try {
      await remote.actualizarPersonaData(personaData);
      return const Right(null);
    } on DioException catch (e) {
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
