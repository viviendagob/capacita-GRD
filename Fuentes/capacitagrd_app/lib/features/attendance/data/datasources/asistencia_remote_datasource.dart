import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/asistencia_model.dart';

abstract class AsistenciaRemoteDataSource {
  Future<List<AsistenciaModel>> getAsistencias(int idEvento, int idPersona);
  Future<AsistenciaModel> registrarAsistencia(RegistrarAsistenciaRequest request);
}

class AsistenciaRemoteDataSourceImpl implements AsistenciaRemoteDataSource {
  final Dio dio;
  const AsistenciaRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<AsistenciaModel>> getAsistencias(int idEvento, int idPersona) async {
    final response = await dio.get('${ApiRoutes.eventosAsistencias}/$idEvento/$idPersona');
    final list = response.data as List? ?? [];
    return list.map((e) => AsistenciaModel.fromJson(e)).toList();
  }

  @override
  Future<AsistenciaModel> registrarAsistencia(RegistrarAsistenciaRequest request) async {
    // EventosAsistenciasEndpoints.Agregar exige además idPersona/idEvento/fecha por query string,
    // aparte del body (mismo patrón que EventosParticipantesEndpoints.Agregar).
    final response = await dio.post(
      ApiRoutes.eventosAsistencias,
      data: request.toJson(),
      queryParameters: {
        'idPersona': request.idPersona,
        'idEvento': request.idEvento,
        'fecha': request.fecha.toIso8601String(),
      },
    );
    return AsistenciaModel.fromJson(response.data);
  }
}
