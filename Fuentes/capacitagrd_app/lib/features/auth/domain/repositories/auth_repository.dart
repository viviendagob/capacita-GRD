import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../entities/auth_entity.dart';

abstract class AuthRepository {
  Future<Either<Failure, AuthEntity>> login(String username, String password);
  Future<Either<Failure, void>> logout();
  Future<bool> isLoggedIn();
  Future<String?> getToken();
}
