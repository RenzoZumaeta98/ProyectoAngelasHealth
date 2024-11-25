﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolAngeSolClinicaHealthla_sHealth.DataAccess;
using SolClinicaHealth.DataAccess.Entities;
using SolClinicaHealth.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolClinicaHealth.Controllers
{
    public class CitaController : Controller
    {
        private readonly AngelasHealthContext _context;
        private readonly IMapper _mapper;

        public CitaController(AngelasHealthContext context, IMapper mapper)
        {
            this._context = context;
            _mapper = mapper;

        }

        public IActionResult AgendarCita(UsuarioViewModel usuario)
        {
            var model = new CitaViewModel();
            model.idUsuario = usuario.IdUsuario.ToString();
            model.Especialidades = _context.Especialidad.ToList();
            model.Doctores = _context.Doctor.ToList();
            return View(model);
        }


        public IActionResult ListarCitas(UsuarioViewModel usuario)
        {
            var citasListDB = _context.Cita.Where(c => c.Usuario.IdUsuario.ToString() == usuario.IdUsuario.ToString()).ToList();

            var list = new List<CitaViewModel>();


            for (int i = 0; i < citasListDB.Count; i++)
            {

                var doctor = (from cita in _context.Cita
                               join user in _context.Doctor
                               on cita.Doctor.IdDoctor equals user.IdDoctor
                               where cita.IdCita.ToString() == citasListDB[i].IdCita.ToString()
                               select user.NombreDoctor + " " + user.ApellidoDoctor ).FirstOrDefault();

                var especialidad = (from cita in _context.Cita
                                 join user in _context.Especialidad
                                 on cita.Especialidad.IdEspecialidad equals user.IdEspecialidad
                                 where cita.IdCita.ToString() == citasListDB[i].IdCita.ToString()
                                 select user.NombreEspecialidad).FirstOrDefault();

                list.Add(new CitaViewModel()
                {
                    IdCita = citasListDB[i].IdCita,
                    idUsuario = usuario.IdUsuario.ToString(),
                    FechaCita = citasListDB[i].FechaCita.ToString(),
                    HoraCita = citasListDB[i].HoraCita.ToString(),
                    EstadoCita = citasListDB[i].EstadoCita.ToString(),


                    idEspecialidad = especialidad,

                    idDoctor = doctor

                }); ;
            }
            return View(list);
        }
                  

        [HttpPost]
        public IActionResult SaveReserva(CitaViewModel citaViewModel)
        {
            var reservaCita = _context.Cita.Add(new CitaEntity()
            {
                Usuario = _context.Usuario.Where(c => c.IdUsuario.ToString() == citaViewModel.idUsuario.ToString()).SingleOrDefault(),
                Especialidad = _context.Especialidad.Where(c => c.IdEspecialidad.ToString() == citaViewModel.idEspecialidad.ToString()).SingleOrDefault(),
                Doctor = _context.Doctor.Where(c => c.IdDoctor.ToString() == citaViewModel.idDoctor.ToString()).SingleOrDefault(),
                FechaCita = DateTime.Parse(citaViewModel.FechaCita),
                HoraCita = citaViewModel.HoraCita,
                EstadoCita = "Reservada"
            });

            _context.SaveChanges();
            return RedirectToAction("CitaReservada", new { idUser = citaViewModel.idUsuario.ToString() });
        }

        public IActionResult CitaReservada(string idUser)
        {
            var idUsuario = idUser;
            var model = _context.Usuario.Where(c => c.IdUsuario.ToString() == idUser).SingleOrDefault();
            var usuarioInView = _mapper.Map<UsuarioViewModel>(model);

            return View(usuarioInView);
        }

        public IActionResult PagarCita(int id)
        {
            var cita = _context.Cita.Where(c => c.IdCita.ToString() == id.ToString()).ToList();
            var citaPagoModel = new CitaPagoViewModel();

            var datos = (from citaN in _context.Cita
                          join doctorN in _context.Doctor on citaN.Doctor.IdDoctor equals doctorN.IdDoctor
                          join userN in _context.Usuario on citaN.Usuario.IdUsuario equals userN.IdUsuario
                          where citaN.IdCita.ToString() == id.ToString()
                          select new {
                              nombrePaciente = userN.NombreUsuario + userN.ApellidoUsuario,
                              documentoPaciente = userN.NroDocumentoUsuario,
                              especialidad = citaN.Especialidad.NombreEspecialidad,
                              fecha = citaN.FechaCita.ToString(),
                              hora = citaN.HoraCita.ToString(),
                              costo = citaN.Especialidad.PrecioConsultaEspecialidad.ToString()
                          }
                          ).FirstOrDefault();


            citaPagoModel.IdCita = id.ToString();
            citaPagoModel.Paciente = datos.nombrePaciente;
            citaPagoModel.DocumentoPaciente = datos.documentoPaciente;
            citaPagoModel.Especialidad = datos.especialidad;
            citaPagoModel.Fecha = datos.fecha;
            citaPagoModel.Hora = datos.hora;
            citaPagoModel.Costo = datos.costo;


            return View(citaPagoModel);
        }


        [HttpPost]
        public IActionResult SavePago(CitaPagoViewModel citaPagoViewModel)
        {
            var CDP = _context.CDP.Add(new CDPEntity()
            {
                Cita = _context.Cita.Where(c => c.IdCita.ToString() == citaPagoViewModel.IdCita.ToString()).SingleOrDefault(),
                PrecioCita = (Double)citaPagoViewModel.PrecioCita,
                EstadoCDP = citaPagoViewModel.EstadoCDP,
                NroTarjeta = citaPagoViewModel.NroTarjeta,
                TipoCDP = citaPagoViewModel.TipoCDP,
                NroFactura = citaPagoViewModel.NroFactura
            });

            var CITA = _context.Cita.FirstOrDefault(c => c.IdCita.ToString() == citaPagoViewModel.IdCita.ToString());
            CITA.EstadoCita = "Pagado";

            var idUser = (from cita in _context.Cita
                          join user in _context.Usuario
                          on cita.Usuario.IdUsuario equals user.IdUsuario
                          where cita.IdCita.ToString() == citaPagoViewModel.IdCita.ToString()
                          select user.IdUsuario).FirstOrDefault();

            var model = _context.Usuario.Where(c => c.IdUsuario.ToString() == idUser.ToString()).SingleOrDefault();
            var usuarioInView = _mapper.Map<UsuarioViewModel>(model);

            _context.SaveChanges();
            return RedirectToAction("ListarCitas", usuarioInView);
        }


        public IActionResult AplazarCita(int id)
        {

            var idCita = id.ToString();
           



            return View();
        }

        public IActionResult CancelarCita(int id)
        {
            var idCita = id.ToString();

            return View();
        }

        public IActionResult ValorarCita(int id)
        {
            var cita = _context.Cita.Where(c => c.IdCita.ToString() == id.ToString()).ToList();
            
            return View(cita);
        }
    }
}
