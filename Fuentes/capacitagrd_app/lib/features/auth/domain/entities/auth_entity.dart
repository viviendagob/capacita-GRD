import 'package:equatable/equatable.dart';

class AuthEntity extends Equatable {
  final String token;
  final DateTime expiration;

  const AuthEntity({required this.token, required this.expiration});

  bool get isExpired => DateTime.now().isAfter(expiration);

  @override
  List<Object?> get props => [token, expiration];
}
