import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/certificado_model.dart';

abstract class CertificadoRepository {
  Future<Either<Failure, List<CertificadoModel>>> getMisCertificados(int idPersona);
  Future<Either<Failure, String>> getUrlCertificado(int idEvento, int idPersona);
}
