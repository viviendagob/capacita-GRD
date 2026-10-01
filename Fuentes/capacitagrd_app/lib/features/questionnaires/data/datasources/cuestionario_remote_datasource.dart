import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/cuestionario_model.dart';

abstract class CuestionarioRemoteDataSource {
  Future<CuestionarioModel> getCuestionario(int idCuestionario);
}

class CuestionarioRemoteDataSourceImpl implements CuestionarioRemoteDataSource {
  final Dio dio;
  const CuestionarioRemoteDataSourceImpl({required this.dio});

  @override
  Future<CuestionarioModel> getCuestionario(int idCuestionario) async {
    final response = await dio.get('${ApiRoutes.cuestionarios}/$idCuestionario');
    return CuestionarioModel.fromJson(response.data);
  }
}
