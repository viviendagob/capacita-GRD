import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/encuesta_repository.dart';
import '../datasources/encuesta_remote_datasource.dart';
import '../datasources/encuesta_local_datasource.dart';
import '../models/encuesta_model.dart';

class EncuestaRepositoryImpl implements EncuestaRepository {
  final EncuestaRemoteDataSource remote;
  final EncuestaLocalDataSource local;

  const EncuestaRepositoryImpl({required this.remote, required this.local});

  @override
  Future<Either<Failure, EncuestaModel>> getEncuesta(int idEncuesta) async {
    try {
      return Right(await remote.getEncuesta(idEncuesta));
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      if (e.response?.statusCode == 404) return Left(NotFoundFailure('Encuesta no encontrada'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> guardarRespuestasLocal(List<RespuestaParticipanteModel> respuestas) async {
    try {
      await local.guardarRespuestas(respuestas);
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, List<RespuestaParticipanteModel>>> getRespuestasLocal(
      int idEncuesta, int idPersona, int idEvento) async {
    try {
      return Right(await local.getRespuestas(idEncuesta, idPersona, idEvento));
    } catch (e) {
      return Left(CacheFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> enviarRespuestas(List<RespuestaParticipanteModel> respuestas) async {
    try {
      await remote.responder(respuestas);
      return const Right(null);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
