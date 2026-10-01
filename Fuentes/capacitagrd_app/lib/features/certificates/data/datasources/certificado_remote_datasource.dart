import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/certificado_model.dart';

abstract class CertificadoRemoteDataSource {
  Future<List<CertificadoModel>> getMisCertificados(int idPersona);
  Future<String> getUrlCertificado(int idEvento, int idPersona);
}

class CertificadoRemoteDataSourceImpl implements CertificadoRemoteDataSource {
  final Dio dio;
  const CertificadoRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<CertificadoModel>> getMisCertificados(int idPersona) async {
    final response = await dio.get('${ApiRoutes.certificados}/$idPersona');
    final list = response.data as List? ?? [];
    return list.map((e) => CertificadoModel.fromJson(e)).toList();
  }

  @override
  Future<String> getUrlCertificado(int idEvento, int idPersona) async {
    final response = await dio.get('${ApiRoutes.certificados}/$idEvento/$idPersona');
    return response.data['url'] ?? response.data.toString();
  }
}
