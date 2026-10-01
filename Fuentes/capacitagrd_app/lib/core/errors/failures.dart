abstract class Failure {
  final String message;
  const Failure(this.message);
}

class NetworkFailure extends Failure {
  const NetworkFailure([String message = 'Sin conexión a internet']) : super(message);
}

class ServerFailure extends Failure {
  final int? statusCode;
  const ServerFailure(String message, {this.statusCode}) : super(message);
}

class AuthFailure extends Failure {
  const AuthFailure([String message = 'Credenciales inválidas']) : super(message);
}

class UnauthorizedFailure extends Failure {
  const UnauthorizedFailure([String message = 'Sesión expirada. Por favor inicie sesión nuevamente.']) : super(message);
}

class NotFoundFailure extends Failure {
  const NotFoundFailure([String message = 'Registro no encontrado']) : super(message);
}

class CacheFailure extends Failure {
  const CacheFailure([String message = 'Error al acceder al almacenamiento local']) : super(message);
}

class ValidationFailure extends Failure {
  const ValidationFailure(String message) : super(message);
}

class GeoFailure extends Failure {
  const GeoFailure([String message = 'No se pudo obtener la ubicación']) : super(message);
}

class BiometricFailure extends Failure {
  const BiometricFailure([String message = 'Autenticación biométrica fallida']) : super(message);
}
