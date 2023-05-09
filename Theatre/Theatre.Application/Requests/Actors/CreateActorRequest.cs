﻿namespace Theatre.Application.Requests.Actors;

public record CreateActorRequest(
    string FirstName, 
    string LastName, 
    string MiddleName, 
    DateTime DateOfBirth, 
    string Dignity,
    double Experience,
    string Email, 
    string Telephone, 
    string Address,
    string Login);