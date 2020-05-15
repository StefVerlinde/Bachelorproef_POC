# Proof of concept Stef Verlinde

Deze projecten werden aangemaakt in functie van de bachelorproef “OAuth 2.0 aan de hand van azure rolling keys”. 

## SecureAPI

Dit is een .NET Core api project. Deze kan gestart worden met het command "dotnet run".

## SecureClient

Dit is een console applicatie bedoeld voor de connectie met de secureAPI en azure active directory te testen. Wanneer de secureAPI actief is
kan deze client met het commando "donet run" een token van Azure active directory halen en zo aan de data op de secureAPI komen. 

## active-direcotry-aspnetcore-webapp

Dit is een quickstart sign-on client van Azure, deze kan gestart worden en vraagt voor een athenticatie met een Microsoft account. Let op!
Enkel met een admin account kan hier worden ingelogd.
