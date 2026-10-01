import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/errors/failures.dart';
import '../../../../core/utils/jwt_utils.dart';
import '../../domain/entities/auth_entity.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource remoteDataSource;
  final FlutterSecureStorage secureStorage;

  const AuthRepositoryImpl({required this.remoteDataSource, required this.secureStorage});

  @override
  Future<Either<Failure, AuthEntity>> login(String username, String password) async {
    try {
      final response = await remoteDataSource.login(LoginRequest(username: username, password: password));
      await secureStorage.write(key: AppConstants.tokenKey, value: response.token);
      final rol = JwtUtils.extraerRol(response.token);
      if (rol != null) {
        await secureStorage.write(key: AppConstants.rolKey, value: rol);
      }
      await _guardarPersonaAsociada(username);
      return Right(AuthEntity(token: response.token, expiration: response.expiration));
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) return Left(AuthFailure('Credenciales incorrectas'));
      return Left(NetworkFailure(e.message ?? 'Error de conexión'));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  // El usuario del login es el num. documento (ver PersonasEndpoints.Agregar en la API,
  // que crea el Usuario con USUARIO = NUM_DOCUMENTO). Con eso resolvemos el idPersona
  // y lo guardamos, para que Perfil/Eventos/Certificados/Encuestas dejen de ver "Sesión inválida".
  Future<void> _guardarPersonaAsociada(String numDocumento) async {
    try {
      final persona = await remoteDataSource.buscarPersonaPorDocumento(numDocumento);
      if (persona == null) return;

      final idPersona = persona['iD_PERSONA'];
      if (idPersona != null) {
        await secureStorage.write(key: AppConstants.personaIdKey, value: idPersona.toString());
      }

      final idPersonaData = persona['iD_PERSONA_DATA'];
      if (idPersonaData != null) {
        await secureStorage.write(key: AppConstants.personaDataIdKey, value: idPersonaData.toString());
      }
    } catch (_) {
      // No bloquear el login si esta resolución falla; solo Perfil quedará sin datos.
    }
  }

  @override
  Future<Either<Failure, void>> logout() async {
    try {
      await remoteDataSource.logout();
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(e.toString()));
    }
  }

  @override
  Future<bool> isLoggedIn() async {
    final token = await secureStorage.read(key: AppConstants.tokenKey);
    return token != null && token.isNotEmpty;
  }

  @override
  Future<String?> getToken() async {
    return secureStorage.read(key: AppConstants.tokenKey);
  }
}
