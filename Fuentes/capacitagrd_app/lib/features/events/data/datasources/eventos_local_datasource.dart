import 'dart:convert';
import 'package:hive_flutter/hive_flutter.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/evento_model.dart';

abstract class EventosLocalDataSource {
  Future<List<EventoParticipanteModel>> getCachedEventos();
  Future<void> cacheEventos(List<EventoParticipanteModel> eventos);
  Future<void> clearCache();
}

class EventosLocalDataSourceImpl implements EventosLocalDataSource {
  final Box box;
  EventosLocalDataSourceImpl({required this.box});

  @override
  Future<List<EventoParticipanteModel>> getCachedEventos() async {
    final cached = box.get(AppConstants.eventosBoxKey);
    if (cached == null) return [];
    final list = jsonDecode(cached) as List;
    return list.map((e) => EventoParticipanteModel.fromJson(e)).toList();
  }

  @override
  Future<void> cacheEventos(List<EventoParticipanteModel> eventos) async {
    final data = eventos.map((e) => e.toJson()).toList();
    await box.put(AppConstants.eventosBoxKey, jsonEncode(data));
  }

  @override
  Future<void> clearCache() async {
    await box.delete(AppConstants.eventosBoxKey);
  }
}
