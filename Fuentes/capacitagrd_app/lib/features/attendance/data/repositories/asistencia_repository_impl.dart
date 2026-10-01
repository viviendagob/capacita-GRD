import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/asistencia_repository.dart';
import '../datasources/asistencia_remote_datasource.dart';
import '../models/asistencia_model.dart';

class AsistenciaRepositoryImpl implements AsistenciaRepository {
  final AsistenciaRemoteDataSource remote;
  const AsistenciaRepositoryImpl({required this.remote});

  @override
  Future<Either<Failure, List<AsistenciaModel>>> getAsistencias(int idEvento, int idPersona) async {
    try {
      final list = await remote.getAsistencias(idEvento, idPersona);
      return Right(list);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, AsistenciaModel>> registrarAsistencia(RegistrarAsistenciaRequest request) async {
    try {
      final result = await remote.registrarAsistencia(request);
      return Right(result);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      if (e.response?.statusCode == 409) return Left(ValidationFailure('Asistencia ya registrada para esta fecha'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
