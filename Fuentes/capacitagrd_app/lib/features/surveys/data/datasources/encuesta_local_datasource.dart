import 'dart:convert';
import 'package:hive_flutter/hive_flutter.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/encuesta_model.dart';

abstract class EncuestaLocalDataSource {
  Future<void> guardarRespuestas(List<RespuestaParticipanteModel> respuestas);
  Future<List<RespuestaParticipanteModel>> getRespuestas(int idEncuesta, int idPersona, int idEvento);
  Future<void> eliminarRespuestas(int idEncuesta, int idPersona, int idEvento);
}

class EncuestaLocalDataSourceImpl implements EncuestaLocalDataSource {
  final Box box;
  EncuestaLocalDataSourceImpl({required this.box});

  String _key(int idEncuesta, int idPersona, int idEvento) =>
      '${AppConstants.encuestasBoxKey}_${idEncuesta}_${idPersona}_$idEvento';

  @override
  Future<void> guardarRespuestas(List<RespuestaParticipanteModel> respuestas) async {
    if (respuestas.isEmpty) return;
    final r = respuestas.first;
    final key = _key(r.idEncuesta, r.idPersona, r.idEvento);
    await box.put(key, jsonEncode(respuestas.map((e) => e.toJson()).toList()));
  }

  @override
  Future<List<RespuestaParticipanteModel>> getRespuestas(int idEncuesta, int idPersona, int idEvento) async {
    final raw = box.get(_key(idEncuesta, idPersona, idEvento));
    if (raw == null) return [];
    final list = jsonDecode(raw) as List;
    return list.map((e) => RespuestaParticipanteModel.fromJson(e)).toList();
  }

  @override
  Future<void> eliminarRespuestas(int idEncuesta, int idPersona, int idEvento) async {
    await box.delete(_key(idEncuesta, idPersona, idEvento));
  }
}
