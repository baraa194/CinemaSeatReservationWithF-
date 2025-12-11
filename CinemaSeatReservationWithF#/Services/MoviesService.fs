namespace CinemaSeatReservationWithFSharp.Services

open CinemaSeatReservationWithFSharp.Models
open CinemaSeatReservationWithFSharp.Repositories

module MoviesService =

    let getAllMovies () = MovieRepo.getAllMovies()
    let getMovieById id = MovieRepo.getMovieById id
    let createMovie movie = MovieRepo.createMovie movie
    let updateMovie movie = MovieRepo.updateMovie movie
    let deleteMovie id = MovieRepo.deleteMovie id

