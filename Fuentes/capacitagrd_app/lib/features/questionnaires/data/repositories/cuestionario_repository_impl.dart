import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/cuestionario_repository.dart';
import '../datasources/cuestionario_remote_datasource.dart';
import '../datasources/cuestionario_local_datasource.dart';
import '../models/cuestionario_model.dart';

class CuestionarioRepositoryImpl implements CuestionarioRepository {
  final CuestionarioRemoteDataSource remote;
  final CuestionarioLocalDataSource local;

  const CuestionarioRepositoryImpl({required this.remote, required this.local});

  @override
  Future<Either<Failure, CuestionarioModel>> getCuestionario(int idCuestionario) async {
    try {
      return Right(await remote.getCuestionario(idCuestionario));
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      if (e.response?.statusCode == 404) return Left(NotFoundFailure('Cuestionario no encontrado'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> guardarRespuestasLocal(List<RespuestaCuestionarioModel> respuestas) async {
    try {
      await local.guardarRespuestas(respuestas);
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, List<RespuestaCuestionarioModel>>> getRespuestasLocal(
      int idCuestionario, int idPersona, int idEvento) async {
    try {
      return Right(await local.getRespuestas(idCuestionario, idPersona, idEvento));
    } catch (e) {
      return Left(CacheFailure(e.toString()));
    }
  }
}
