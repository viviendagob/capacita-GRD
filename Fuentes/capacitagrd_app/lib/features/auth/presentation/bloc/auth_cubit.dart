import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';
import '../../domain/repositories/auth_repository.dart';

part 'auth_state.dart';

class AuthCubit extends Cubit<AuthState> {
  final AuthRepository repository;

  AuthCubit({required this.repository}) : super(AuthInitial());

  Future<void> checkAuth() async {
    final loggedIn = await repository.isLoggedIn();
    emit(loggedIn ? AuthAuthenticated() : AuthUnauthenticated());
  }

  Future<void> login(String username, String password) async {
    emit(AuthLoading());
    final result = await repository.login(username, password);
    result.fold(
      (failure) => emit(AuthError(failure.message)),
      (_) => emit(AuthAuthenticated()),
    );
  }

  Future<void> logout() async {
    await repository.logout();
    emit(AuthUnauthenticated());
  }
}
