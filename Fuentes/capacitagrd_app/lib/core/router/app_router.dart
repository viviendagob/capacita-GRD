import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/presentation/bloc/auth_cubit.dart';
import '../../features/auth/presentation/pages/login_page.dart';
import '../../features/auth/presentation/pages/splash_page.dart';
import '../../features/auth/presentation/pages/registro_page.dart';
import '../../features/events/presentation/pages/eventos_page.dart';
import '../../features/events/presentation/pages/eventos_disponibles_page.dart';
import '../../features/events/presentation/pages/evento_detalle_page.dart';
import '../../features/attendance/presentation/pages/asistencia_page.dart';
import '../../features/surveys/presentation/pages/encuesta_page.dart';
import '../../features/questionnaires/presentation/pages/cuestionario_page.dart';
import '../../features/certificates/presentation/pages/certificados_page.dart';
import '../../features/profile/presentation/pages/perfil_page.dart';
import '../di/injection.dart';
import '../../shared/widgets/main_scaffold.dart';

// BLoC providers wired in ShellRoute
import '../../features/events/presentation/bloc/eventos_cubit.dart';
import '../../features/attendance/presentation/bloc/asistencia_cubit.dart';
import '../../features/profile/presentation/bloc/persona_cubit.dart';
import '../../features/certificates/presentation/bloc/certificado_cubit.dart';
import '../../features/surveys/presentation/bloc/encuesta_cubit.dart';
import '../../features/questionnaires/presentation/bloc/cuestionario_cubit.dart';

final _rootNavKey = GlobalKey<NavigatorState>();
final _shellNavKey = GlobalKey<NavigatorState>();

class AppRouter {
  final AuthCubit authCubit;

  AppRouter(this.authCubit);

  late final GoRouter router = GoRouter(
    navigatorKey: _rootNavKey,
    initialLocation: '/',
    refreshListenable: GoRouterRefreshStream(authCubit.stream),
    redirect: (context, state) {
      final auth = authCubit.state;
      final loc = state.matchedLocation;
      final publicRoutes = ['/', '/login', '/registro'];

      if (auth is AuthUnauthenticated && !publicRoutes.contains(loc)) return '/login';
      if (auth is AuthAuthenticated && (loc == '/' || loc == '/login')) return '/eventos';
      return null;
    },
    routes: [
      GoRoute(path: '/', builder: (_, __) => const SplashPage()),
      GoRoute(path: '/login', builder: (_, __) => const LoginPage()),
      GoRoute(path: '/registro', builder: (_, __) => const RegistroPage()),
      ShellRoute(
        navigatorKey: _shellNavKey,
        builder: (context, state, child) => MultiBlocProvider(
          providers: [
            BlocProvider(create: (_) => getIt<EventosCubit>()),
            BlocProvider(create: (_) => getIt<PersonaCubit>()),
            BlocProvider(create: (_) => getIt<CertificadoCubit>()),
          ],
          child: MainScaffold(child: child),
        ),
        routes: [
          GoRoute(
            path: '/eventos',
            pageBuilder: (_, __) => const NoTransitionPage(child: EventosPage()),
            routes: [
              GoRoute(
                path: ':id',
                builder: (_, state) => EventoDetallePage(
                    idEvento: int.parse(state.pathParameters['id']!)),
              ),
            ],
          ),
          GoRoute(
            path: '/eventos-disponibles',
            pageBuilder: (_, __) => const NoTransitionPage(child: EventosDisponiblesPage()),
          ),
          GoRoute(
            path: '/perfil',
            pageBuilder: (_, __) => const NoTransitionPage(child: PerfilPage()),
          ),
          GoRoute(
            path: '/certificados',
            pageBuilder: (_, __) => const NoTransitionPage(child: CertificadosPage()),
          ),
        ],
      ),
      // Routes outside shell (full screen)
      GoRoute(
        path: '/validar-asistencia/:idEvento',
        builder: (_, state) => BlocProvider(
          create: (_) => getIt<AsistenciaCubit>(),
          child: AsistenciaPage(idEvento: int.parse(state.pathParameters['idEvento']!)),
        ),
      ),
      GoRoute(
        path: '/encuesta/:idEncuesta/:idEvento',
        builder: (_, state) => BlocProvider(
          create: (_) => getIt<EncuestaCubit>(),
          child: EncuestaPage(
            idEncuesta: int.parse(state.pathParameters['idEncuesta']!),
            idEvento: int.parse(state.pathParameters['idEvento']!),
          ),
        ),
      ),
      GoRoute(
        path: '/cuestionario/:idCuestionario/:idEvento',
        builder: (_, state) => BlocProvider(
          create: (_) => getIt<CuestionarioCubit>(),
          child: CuestionarioPage(
            idCuestionario: int.parse(state.pathParameters['idCuestionario']!),
            idEvento: int.parse(state.pathParameters['idEvento']!),
          ),
        ),
      ),
    ],
  );
}

class GoRouterRefreshStream extends ChangeNotifier {
  GoRouterRefreshStream(Stream stream) {
    _sub = stream.listen((_) => notifyListeners());
  }
  late final dynamic _sub;

  @override
  void dispose() {
    _sub.cancel();
    super.dispose();
  }
}
