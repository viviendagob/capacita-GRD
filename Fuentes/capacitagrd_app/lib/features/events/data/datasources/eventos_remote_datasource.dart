import 'package:dio/dio.dart';
import '../../../../core/constants/app_constants.dart';
import '../models/evento_model.dart';

abstract class EventosRemoteDataSource {
  Future<List<EventoParticipanteModel>> getMisEventos(int idPersona);
  Future<List<EventoModel>> getEventosDisponibles();
  Future<EventoModel> getEvento(int idEvento);
  Future<EventoParticipanteModel> inscribirseEvento({
    required int idEvento,
    required int idPersona,
    required int idPersonaData,
    required int idModalidad,
    double? latitud,
    double? longitud,
  });
  // null si no está inscrito; si lo está, trae también su QR personal (codigoQr/qrUrl) si el
  // evento genera ticket.
  Future<EventoParticipanteModel?> verificarInscripcion(int idEvento, int idPersona);
}

class EventosRemoteDataSourceImpl implements EventosRemoteDataSource {
  final Dio dio;
  const EventosRemoteDataSourceImpl({required this.dio});

  @override
  Future<List<EventoParticipanteModel>> getMisEventos(int idPersona) async {
    // GET /eventosparticipantes/eventos?idPersona= (EventosParticipantesEndpoints.Eventos)
    final response = await dio.get(
      ApiRoutes.misEventos,
      queryParameters: {'idPersona': idPersona},
    );
    final list = response.data as List? ?? [];
    return list.map((e) => EventoParticipanteModel.fromJson(e)).toList();
  }

  @override
  Future<List<EventoModel>> getEventosDisponibles() async {
    // GET /eventos (EventosEndpoints.Listar) — todos los eventos, para que el participante
    // pueda descubrirlos e inscribirse (antes no existía ninguna pantalla para esto).
    final response = await dio.get(ApiRoutes.eventos);
    final list = response.data as List? ?? [];
    return list.map((e) => EventoModel.fromJson(e)).toList();
  }

  @override
  Future<EventoModel> getEvento(int idEvento) async {
    // GET /eventos/{id} (EventosEndpoints.Obtener); no confundir con /eventosparticipantes
    final response = await dio.get('${ApiRoutes.eventos}/$idEvento');
    return EventoModel.fromJson(response.data);
  }

  @override
  Future<EventoParticipanteModel> inscribirseEvento({
    required int idEvento,
    required int idPersona,
    required int idPersonaData,
    required int idModalidad,
    double? latitud,
    double? longitud,
  }) async {
    final body = EventoParticipanteModel(
      idEvento: idEvento,
      idPersona: idPersona,
      idPersonaData: idPersonaData,
      idModalidad: idModalidad,
      fechaReg: DateTime.now(),
      latitud: latitud,
      longitud: longitud,
    );
    // EventosParticipantesEndpoints.Agregar además del body exige idPersona/idEvento por query string.
    final response = await dio.post(
      ApiRoutes.eventosParticipantes,
      data: body.toJson(),
      queryParameters: {'idPersona': idPersona, 'idEvento': idEvento},
    );
    return EventoParticipanteModel.fromJson(response.data);
  }

  @override
  Future<EventoParticipanteModel?> verificarInscripcion(int idEvento, int idPersona) async {
    // GET /eventosparticipantes?idEvento=&idPersona= (EventosParticipantesEndpoints.Existe)
    try {
      final response = await dio.get(
        ApiRoutes.eventosParticipantes,
        queryParameters: {'idEvento': idEvento, 'idPersona': idPersona},
      );
      return EventoParticipanteModel.fromJson(response.data);
    } on DioException catch (e) {
      if (e.response?.statusCode == 404) return null;
      rethrow;
    }
  }
}
