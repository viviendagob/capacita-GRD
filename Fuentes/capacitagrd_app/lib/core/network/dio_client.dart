import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../constants/app_constants.dart';

class DioClient {
  late final Dio _dio;
  final FlutterSecureStorage _secureStorage;

  DioClient(this._secureStorage) {
    _dio = Dio(
      BaseOptions(
        baseUrl: AppConstants.baseUrl,
        connectTimeout: const Duration(milliseconds: AppConstants.connectTimeoutMs),
        receiveTimeout: const Duration(milliseconds: AppConstants.receiveTimeoutMs),
        headers: {'Content-Type': 'application/json', 'Accept': 'application/json'},
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await _secureStorage.read(key: AppConstants.tokenKey);
          if (token != null) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (error, handler) async {
          if (error.response?.statusCode == 401) {
            // Token expirado - limpiar sesión
            await _secureStorage.delete(key: AppConstants.tokenKey);
            await _secureStorage.delete(key: AppConstants.tokenExpiryKey);
          }
          handler.next(error);
        },
      ),
    );

    _dio.interceptors.add(LogInterceptor(
      requestBody: true,
      responseBody: true,
      logPrint: (o) => null, // desactivar en producción
    ));
  }

  Dio get dio => _dio;

  Future<Response> get(String path, {Map<String, dynamic>? queryParameters}) =>
      _dio.get(path, queryParameters: queryParameters);

  Future<Response> post(String path, {dynamic data, Map<String, dynamic>? queryParameters}) =>
      _dio.post(path, data: data, queryParameters: queryParameters);

  Future<Response> put(String path, {dynamic data, Map<String, dynamic>? queryParameters}) =>
      _dio.put(path, data: data, queryParameters: queryParameters);

  Future<Response> delete(String path, {Map<String, dynamic>? queryParameters}) =>
      _dio.delete(path, queryParameters: queryParameters);
}
