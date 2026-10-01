import 'dart:convert';
import 'package:hive_flutter/hive_flutter.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/cuestionario_model.dart';

abstract class CuestionarioLocalDataSource {
  Future<void> guardarRespuestas(List<RespuestaCuestionarioModel> respuestas);
  Future<List<RespuestaCuestionarioModel>> getRespuestas(int idCuestionario, int idPersona, int idEvento);
  Future<void> eliminarRespuestas(int idCuestionario, int idPersona, int idEvento);
}

class CuestionarioLocalDataSourceImpl implements CuestionarioLocalDataSource {
  final Box box;
  CuestionarioLocalDataSourceImpl({required this.box});

  String _key(int idCuestionario, int idPersona, int idEvento) =>
      '${AppConstants.cuestionariosBoxKey}_${idCuestionario}_${idPersona}_$idEvento';

  @override
  Future<void> guardarRespuestas(List<RespuestaCuestionarioModel> respuestas) async {
    if (respuestas.isEmpty) return;
    final r = respuestas.first;
    await box.put(_key(r.idCuestionario, r.idPersona, r.idEvento),
        jsonEncode(respuestas.map((e) => e.toJson()).toList()));
  }

  @override
  Future<List<RespuestaCuestionarioModel>> getRespuestas(
      int idCuestionario, int idPersona, int idEvento) async {
    final raw = box.get(_key(idCuestionario, idPersona, idEvento));
    if (raw == null) return [];
    final list = jsonDecode(raw) as List;
    return list
        .map((e) => RespuestaCuestionarioModel(
              idCuestionario: e['idCuestionario'],
              idPersona: e['idPersona'],
              idEvento: e['idEvento'],
              idPregunta: e['idPregunta'],
              idRespuestaSeleccionada: e['idRespuestaSeleccionada'],
              fechaRespuesta: DateTime.parse(e['fechaRespuesta']),
            ))
        .toList();
  }

  @override
  Future<void> eliminarRespuestas(int idCuestionario, int idPersona, int idEvento) async {
    await box.delete(_key(idCuestionario, idPersona, idEvento));
  }
}
