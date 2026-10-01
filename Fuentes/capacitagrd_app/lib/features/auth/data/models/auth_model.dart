import 'dart:convert';
import 'package:crypto/crypto.dart';
import 'package:equatable/equatable.dart';

// POST /auth/login  → body: LoginRequestDTO
class LoginRequest {
  final String username;
  final String password;
  const LoginRequest({required this.username, required this.password});

  // La API compara contra CLAVE = Convert.ToBase64String(SHA256(password)) tal cual
  // la guarda PersonasEndpoints.Agregar; /auth/login no hashea nada de su lado
  // (ver AuthEndpoints.Login), así que hay que enviarla ya hasheada igual que
  // LoginController.EncriptarClave en el Admin.
  static String _hashClave(String clave) => base64.encode(sha256.convert(utf8.encode(clave)).bytes);

  Map<String, dynamic> toJson() => {'Username': username, 'Password': _hashClave(password)};
}

// Response: AuthResponseDTO
class AuthResponse extends Equatable {
  final String token;
  final DateTime expiration;

  const AuthResponse({required this.token, required this.expiration});

  factory AuthResponse.fromJson(Map<String, dynamic> json) => AuthResponse(
        token: json['token'] as String,
        expiration: DateTime.parse(json['expiration'] as String),
      );

  @override
  List<Object?> get props => [token, expiration];
}
