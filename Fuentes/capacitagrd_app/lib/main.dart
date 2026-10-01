import 'package:flutter/material.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:firebase_core/firebase_core.dart';
import 'core/constants/app_constants.dart';
import 'core/di/injection.dart';
import 'app.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Hive init
  await Hive.initFlutter();
  await Hive.openBox(AppConstants.authBox);
  await Hive.openBox(AppConstants.eventsBox);
  await Hive.openBox(AppConstants.attendanceBox);
  await Hive.openBox(AppConstants.surveysBox);
  await Hive.openBox(AppConstants.questionnairesBox);
  await Hive.openBox(AppConstants.personaBox);
  await Hive.openBox(AppConstants.pendingSyncBox);

  // Firebase (opcional si no hay google-services.json aún)
  try {
    await Firebase.initializeApp();
  } catch (_) {}

  // Dependency injection
  await configureDependencies();

  runApp(const CapacitaGrdApp());
}
