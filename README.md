# EVOLTIS-backend-Pereyra

Proyecto de Usuarios realizado por Juan Cruz Pereyra.

Explicacion breve de la arquitectura empleada y la descripcion de tecnologias utilizadas:

Dividi el proyectos en diferentes capas:

### UserApi:
Proyecto de puerta de entrada para la solucion. Creacion de endpoints CRUD para Usuarios ejecutados visualmente con la interfaz de swagger:
Aqui tambien se realizo:
- Configuracion de Servicios
- Configuracion de EntityFramework y base de datos MySQL
- Configuracion de FluentValidation
- Utilizacion de Mappers con AutoMapper
- Configuracion de un manejo global de excepciones

### UserApplication:
Capa encargada de la logica de negocio y la gestion de usuarios. En el pude implementar tenoclogias como AutoMapper para mapear DTO con entidades de dominio y FluentValidations para validar la entrada de datos.
Depende de la capa de UserDomain para acceder a las entidades y UserInfrastructure para el acceso a los repositorios.

Pude incluir expeciones customizadas y utilidades como objetos de paginado. 


### UserDomain:
Capa donde modelo las entidades principales del sitema con reglas de negocio basicas. Tambien es el modelo de la base de datos. Puedo agregar que, si se tratara de un proyecto con escalabilidad, se podria haber optado por separar el modelo de la base de datos independiente a las entidades de dominio, para no acoplar el esquema de la base de datos a nuesto dominio.

Defino las entidades principales del proyecto: Usuario y Domicilio. Tambien, sus relaciones y comportamientos.

### UserInfrastructure:
Capa de acceso a datos, donde utilizo EntityFramework como ORM y MySQL.

El enfoque es un codefirst, por lo que se genera una migracion automatica cuando corremos la solucion. 

Configuro un DbContest donde defino las tablas, seteo las restricciones y relaciones de las tablas de mi BD.

Cree los repositorios de acceso para manipular la data persistente.
	
### UserTest: 

Proyecto de pruebas unitarios para diferentes casos. Utilice recursos como FluentAssertions, Testcontainares y xunit.runner entre otros.
Cree distintas pruebas para los endpoints disponibles tanto en casos de exito como no. Tambien pude verificar ciertos flujos de comportamiento simulando el de un usuario.
El proyecto levanta una bd en un container que vive en la ejecucion del mismo de manera local.
Tambien, se alimenta de archivos .json que sirven como mocks tanto para nutrir la base de datos como para emular request en los endpoints de prueba.



--------------------------------------------------------------------------

Para ejecutar la aplicacion se puede hacer de dos maneras una vez clonado el repositorio

### Docker

Cree un archivo docker-compose.yml donde:
- Levanta una instancia de la api del proyecto
- Levanta una instancia de la base de datos con MySQL
- Levanta una instancioa de adminer como manager web de la base de datos

Hay que tener en cuenta ciertas cosas:
- Tener instalado y corriendo docker 
- En lo posible, evitar carpetas de Windows con OneDrive ya que hay problemas con el volumen que genera el docker compose
- En la ruta del repositorio, correr este comando:
	
	`docker compose -f .\docker-compose.yml up -d --build --force-recreate`
	
Esto va a buildear lo necesario para poder ejecutar de manera local los endpoints y ademas poder visualizar la base de datos por medio de adminer
Una corriendo todos los servicios, podemos entrar a 

- Abrir swagger: http://localhost:5000/swagger/index.html
- Abrir adminer:http://localhost:8080
- Las credenciales de acceso se encuentran en el archivo .env del proyecto.
	
### Manual

Tener instalado:
- .NET SDK 8.0.401 
- MySQL Server 8.x
	
Configurar la connection string en appsetings.json, para la BD local. Cambiar los siguientes valores por los que correspondan:
- Database=nombre BD
- User ID= app user Id
- Password= clave de usuario

Comandos:
- `dotnet restore`
- `dotnet build`
- Aplicar migraciones a la BD:
  	`dotnet ef database update -p UserInfrastructure -s UserApi`
- Ejecutar la API:
  ` dotnet run --project .\UserApi`

Consejos utilies:
Si no conecta la BD:
- Verificar que este corriendo el servicio MySQL80
- Verificar que el puerto 3306 este disponible
- Verificar credenciales en connection string

