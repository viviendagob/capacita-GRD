import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/auth_model.dart';

abstract class AuthRemoteDataSource {
  Future<AuthResponse> login(LoginRequest request);
  Future<void> logout();

  // El login solo autentica (usuario = num. documento) y no retorna el idPersona.
  // Se resuelve aparte contra /personas/existe usando el mismo num. documento.
  Future<Map<String, dynamic>?> buscarPersonaPorDocumento(String numDocumento);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio dio;
  final FlutterSecureStorage secureStorage;

  const AuthRemoteDataSourceImpl({required this.dio, required this.secureStorage});

  @override
  Future<AuthResponse> login(LoginRequest request) async {
    final response = await dio.post(
      ApiRoutes.login,
      data: request.toJson(),
    );
    return AuthResponse.fromJson(response.data);
  }

  @override
  Future<Map<String, dynamic>?> buscarPersonaPorDocumento(String numDocumento) async {
    try {
      // ID_TIPO_DOCUMENTO 1 = DNI (mismo valor usado al registrar, ver registro_page.dart)
      final response = await dio.get(
        ApiRoutes.personasExiste,
        queryParameters: {'tipo': 1, 'documento': numDocumento},
      );
      return response.data as Map<String, dynamic>;
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return null;
      rethrow;
    }
  }

  @override
  Future<void> logout() async {
    await secureStorage.delete(key: AppConstants.tokenKey);
    await secureStorage.delete(key: AppConstants.personaIdKey);
    await secureStorage.delete(key: AppConstants.personaDataIdKey);
    await secureStorage.delete(key: AppConstants.rolKey);
  }
}
