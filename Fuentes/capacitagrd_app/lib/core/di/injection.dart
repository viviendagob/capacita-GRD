import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:dio/dio.dart';

import '../constants/app_constants.dart';
import '../network/dio_client.dart';

import '../../features/auth/data/datasources/auth_remote_datasource.dart';
import '../../features/auth/data/repositories/auth_repository_impl.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../../features/auth/presentation/bloc/auth_cubit.dart';

import '../../features/events/data/datasources/eventos_remote_datasource.dart';
import '../../features/events/data/datasources/eventos_local_datasource.dart';
import '../../features/events/data/repositories/eventos_repository_impl.dart';
import '../../features/events/domain/repositories/eventos_repository.dart';
import '../../features/events/presentation/bloc/eventos_cubit.dart';

import '../../features/attendance/data/datasources/asistencia_remote_datasource.dart';
import '../../features/attendance/data/repositories/asistencia_repository_impl.dart';
import '../../features/attendance/domain/repositories/asistencia_repository.dart';
import '../../features/attendance/presentation/bloc/asistencia_cubit.dart';

import '../../features/surveys/data/datasources/encuesta_remote_datasource.dart';
import '../../features/surveys/data/datasources/encuesta_local_datasource.dart';
import '../../features/surveys/data/repositories/encuesta_repository_impl.dart';
import '../../features/surveys/domain/repositories/encuesta_repository.dart';
import '../../features/surveys/presentation/bloc/encuesta_cubit.dart';

import '../../features/questionnaires/data/datasources/cuestionario_remote_datasource.dart';
import '../../features/questionnaires/data/datasources/cuestionario_local_datasource.dart';
import '../../features/questionnaires/data/repositories/cuestionario_repository_impl.dart';
import '../../features/questionnaires/domain/repositories/cuestionario_repository.dart';
import '../../features/questionnaires/presentation/bloc/cuestionario_cubit.dart';

import '../../features/profile/data/datasources/persona_remote_datasource.dart';
import '../../features/profile/data/repositories/persona_repository_impl.dart';
import '../../features/profile/domain/repositories/persona_repository.dart';
import '../../features/profile/presentation/bloc/persona_cubit.dart';

import '../../features/certificates/data/datasources/certificado_remote_datasource.dart';
import '../../features/certificates/data/repositories/certificado_repository_impl.dart';
import '../../features/certificates/domain/repositories/certificado_repository.dart';
import '../../features/certificates/presentation/bloc/certificado_cubit.dart';

import '../../shared/data/maestros_remote_datasource.dart';

final getIt = GetIt.instance;

Future<void> configureDependencies() async {
  // Secure storage
  const secureStorage = FlutterSecureStorage(
    aOptions: AndroidOptions(encryptedSharedPreferences: true),
    iOptions: IOSOptions(accessibility: KeychainAccessibility.first_unlock),
  );
  getIt.registerLazySingleton<FlutterSecureStorage>(() => secureStorage);

  // Dio
  getIt.registerLazySingleton<Dio>(() => DioClient(secureStorage).dio);

  // Hive boxes (already opened in main.dart)
  getIt.registerLazySingleton<Box>(() => Hive.box(AppConstants.eventsBox), instanceName: 'eventsBox');
  getIt.registerLazySingleton<Box>(() => Hive.box(AppConstants.surveysBox), instanceName: 'surveysBox');
  getIt.registerLazySingleton<Box>(() => Hive.box(AppConstants.questionnairesBox), instanceName: 'questionnairesBox');

  // Auth
  getIt.registerLazySingleton<AuthRemoteDataSource>(
      () => AuthRemoteDataSourceImpl(dio: getIt<Dio>(), secureStorage: getIt()));
  getIt.registerLazySingleton<AuthRepository>(
      () => AuthRepositoryImpl(remoteDataSource: getIt(), secureStorage: getIt()));
  getIt.registerFactory<AuthCubit>(() => AuthCubit(repository: getIt()));

  // Events
  getIt.registerLazySingleton<EventosRemoteDataSource>(() => EventosRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<EventosLocalDataSource>(
      () => EventosLocalDataSourceImpl(box: getIt(instanceName: 'eventsBox')));
  getIt.registerLazySingleton<EventosRepository>(
      () => EventosRepositoryImpl(remote: getIt(), local: getIt()));
  getIt.registerFactory<EventosCubit>(() => EventosCubit(repository: getIt()));

  // Attendance
  getIt.registerLazySingleton<AsistenciaRemoteDataSource>(
      () => AsistenciaRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<AsistenciaRepository>(
      () => AsistenciaRepositoryImpl(remote: getIt()));
  getIt.registerFactory<AsistenciaCubit>(() => AsistenciaCubit(repository: getIt(), eventosRepository: getIt()));

  // Surveys
  getIt.registerLazySingleton<EncuestaRemoteDataSource>(
      () => EncuestaRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<EncuestaLocalDataSource>(
      () => EncuestaLocalDataSourceImpl(box: getIt(instanceName: 'surveysBox')));
  getIt.registerLazySingleton<EncuestaRepository>(
      () => EncuestaRepositoryImpl(remote: getIt(), local: getIt()));
  getIt.registerFactory<EncuestaCubit>(() => EncuestaCubit(repository: getIt()));

  // Questionnaires
  getIt.registerLazySingleton<CuestionarioRemoteDataSource>(
      () => CuestionarioRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<CuestionarioLocalDataSource>(
      () => CuestionarioLocalDataSourceImpl(box: getIt(instanceName: 'questionnairesBox')));
  getIt.registerLazySingleton<CuestionarioRepository>(
      () => CuestionarioRepositoryImpl(remote: getIt(), local: getIt()));
  getIt.registerFactory<CuestionarioCubit>(() => CuestionarioCubit(repository: getIt()));

  // Profile
  getIt.registerLazySingleton<PersonaRemoteDataSource>(
      () => PersonaRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<PersonaRepository>(
      () => PersonaRepositoryImpl(remote: getIt()));
  getIt.registerFactory<PersonaCubit>(
      () => PersonaCubit(repository: getIt(), secureStorage: getIt()));

  // Certificates
  getIt.registerLazySingleton<CertificadoRemoteDataSource>(
      () => CertificadoRemoteDataSourceImpl(dio: getIt()));
  getIt.registerLazySingleton<CertificadoRepository>(
      () => CertificadoRepositoryImpl(remote: getIt()));
  getIt.registerFactory<CertificadoCubit>(() => CertificadoCubit(repository: getIt()));

  // Maestros (listas para el formulario de registro: países, entidades, cargos, profesiones, distritos)
  getIt.registerLazySingleton<MaestrosRemoteDataSource>(() => MaestrosRemoteDataSourceImpl(dio: getIt()));
}
