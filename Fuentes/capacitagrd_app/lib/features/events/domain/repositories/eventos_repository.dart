import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../../events/data/models/evento_model.dart';

abstract class EventosRepository {
  Future<Either<Failure, List<EventoParticipanteModel>>> getMisEventos(int idPersona);
  Future<Either<Failure, List<EventoModel>>> getEventosDisponibles();
  Future<Either<Failure, EventoModel>> getEvento(int idEvento);
  Future<Either<Failure, EventoParticipanteModel>> inscribirseEvento({
    required int idEvento,
    required int idPersona,
    required int idPersonaData,
    required int idModalidad,
    double? latitud,
    double? longitud,
  });
  Future<Either<Failure, EventoParticipanteModel?>> verificarInscripcion(int idEvento, int idPersona);
}
