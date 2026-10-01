import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../domain/repositories/certificado_repository.dart';
import '../datasources/certificado_remote_datasource.dart';
import '../models/certificado_model.dart';

class CertificadoRepositoryImpl implements CertificadoRepository {
  final CertificadoRemoteDataSource remote;
  const CertificadoRepositoryImpl({required this.remote});

  @override
  Future<Either<Failure, List<CertificadoModel>>> getMisCertificados(int idPersona) async {
    try {
      return Right(await remote.getMisCertificados(idPersona));
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(UnauthorizedFailure());
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, String>> getUrlCertificado(int idEvento, int idPersona) async {
    try {
      return Right(await remote.getUrlCertificado(idEvento, idPersona));
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return Left(NotFoundFailure('Certificado no disponible'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
