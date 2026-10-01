import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:local_auth/local_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/di/injection.dart';
import '../bloc/auth_cubit.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final _userCtrl = TextEditingController();
  final _passCtrl = TextEditingController();
  bool _obscure = true;
  bool _recordar = false;
  final _auth = LocalAuthentication();
  final _storage = getIt<FlutterSecureStorage>();

  @override
  void initState() {
    super.initState();
    _cargarCredenciales();
  }

  Future<void> _cargarCredenciales() async {
    final user = await _storage.read(key: AppConstants.usernameKey);
    final pass = await _storage.read(key: AppConstants.passwordKey);
    if (user != null) {
      _userCtrl.text = user;
      setState(() => _recordar = true);
      if (pass != null) {
        _passCtrl.text = pass;
        _intentarBiometrico();
      }
    }
  }

  Future<void> _intentarBiometrico() async {
    final biometricHabilitado = await _storage.read(key: AppConstants.biometricEnabledKey);
    if (biometricHabilitado != 'true') return;
    try {
      final canCheck = await _auth.canCheckBiometrics;
      if (!canCheck) return;
      final autenticado = await _auth.authenticate(
        localizedReason: 'Usa tu huella para ingresar',
        options: const AuthenticationOptions(biometricOnly: true),
      );
      if (autenticado && mounted) {
        context.read<AuthCubit>().login(_userCtrl.text, _passCtrl.text);
      }
    } catch (_) {}
  }

  Future<void> _login() async {
    if (!_formKey.currentState!.validate()) return;
    if (_recordar) {
      await _storage.write(key: AppConstants.usernameKey, value: _userCtrl.text);
      await _storage.write(key: AppConstants.passwordKey, value: _passCtrl.text);
    } else {
      await _storage.delete(key: AppConstants.usernameKey);
      await _storage.delete(key: AppConstants.passwordKey);
    }
    if (mounted) {
      context.read<AuthCubit>().login(_userCtrl.text, _passCtrl.text);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF003865),
      body: BlocConsumer<AuthCubit, AuthState>(
        listener: (context, state) {
          if (state is AuthAuthenticated) context.go('/eventos');
          if (state is AuthError) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.message), backgroundColor: Colors.red),
            );
          }
        },
        builder: (context, state) {
          return SafeArea(
            child: Center(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(32),
                child: Column(
                  children: [
                    const Icon(Icons.shield, size: 80, color: Color(0xFFE8A020)),
                    const SizedBox(height: 16),
                    const Text('CAPACITA-GRD',
                        style: TextStyle(color: Colors.white, fontSize: 28, fontWeight: FontWeight.bold, letterSpacing: 2)),
                    const SizedBox(height: 8),
                    const Text('Sistema de Capacitación GRD - MVCS',
                        style: TextStyle(color: Colors.white70, fontSize: 13), textAlign: TextAlign.center),
                    const SizedBox(height: 48),
                    Card(
                      child: Padding(
                        padding: const EdgeInsets.all(24),
                        child: Form(
                          key: _formKey,
                          child: Column(
                            children: [
                              TextFormField(
                                controller: _userCtrl,
                                decoration: const InputDecoration(
                                  labelText: 'Usuario (DNI o usuario de staff)',
                                  prefixIcon: Icon(Icons.person),
                                ),
                                keyboardType: TextInputType.text,
                                autocorrect: false,
                                validator: (v) => v == null || v.isEmpty ? 'Ingresa tu usuario' : null,
                              ),
                              const SizedBox(height: 16),
                              TextFormField(
                                controller: _passCtrl,
                                obscureText: _obscure,
                                decoration: InputDecoration(
                                  labelText: 'Contraseña',
                                  prefixIcon: const Icon(Icons.lock),
                                  suffixIcon: IconButton(
                                    icon: Icon(_obscure ? Icons.visibility : Icons.visibility_off),
                                    onPressed: () => setState(() => _obscure = !_obscure),
                                  ),
                                ),
                                validator: (v) => v == null || v.isEmpty ? 'Ingresa tu contraseña' : null,
                              ),
                              const SizedBox(height: 8),
                              CheckboxListTile(
                                value: _recordar,
                                onChanged: (v) => setState(() => _recordar = v ?? false),
                                title: const Text('Recordar sesión'),
                                controlAffinity: ListTileControlAffinity.leading,
                                contentPadding: EdgeInsets.zero,
                              ),
                              const SizedBox(height: 16),
                              SizedBox(
                                width: double.infinity,
                                child: FilledButton(
                                  onPressed: state is AuthLoading ? null : _login,
                                  child: state is AuthLoading
                                      ? const SizedBox(
                                          height: 20,
                                          width: 20,
                                          child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                                      : const Text('Ingresar'),
                                ),
                              ),
                              const SizedBox(height: 12),
                              TextButton(
                                onPressed: () => context.push('/registro'),
                                child: const Text('¿Primera vez? Regístrate aquí'),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          );
        },
      ),
    );
  }

  @override
  void dispose() {
    _userCtrl.dispose();
    _passCtrl.dispose();
    super.dispose();
  }
}
