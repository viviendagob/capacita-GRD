import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'core/di/injection.dart';
import 'core/router/app_router.dart';
import 'core/theme/app_theme.dart';
import 'features/auth/presentation/bloc/auth_cubit.dart';

class CapacitaGrdApp extends StatelessWidget {
  const CapacitaGrdApp({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => getIt<AuthCubit>()..checkAuth(),
      child: Builder(builder: (context) {
        final router = AppRouter(context.read<AuthCubit>()).router;
        return MaterialApp.router(
          title: 'CAPACITA-GRD',
          theme: AppTheme.light(),
          darkTheme: AppTheme.dark(),
          themeMode: ThemeMode.light,
          routerConfig: router,
          debugShowCheckedModeBanner: false,
        );
      }),
    );
  }
}
