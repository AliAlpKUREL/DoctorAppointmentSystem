# Hata ayıklama kapsayıcınızı özelleştirme ve Visual Studio’nun daha hızlı hata ayıklama için görüntülerinizi derlemek üzere bu Dockerfile'ı nasıl kullandığı hakkında bilgi edinmek için https://aka.ms/customizecontainer sayfasına bakın.

# Bu aşama, VS'den hızlı modda çalıştırıldığında kullanılır (Hata ayıklama yapılandırması için varsayılan olarak ayarlıdır)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# Bu aşama, hizmet projesini oluşturmak için kullanılır
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DoctorAppointmentSystem.WorkerService/DoctorAppointmentSystem.WorkerService.csproj", "DoctorAppointmentSystem.WorkerService/"]
COPY ["DoctorAppointmentSystem.Service/DoctorAppointmentSystem.Service.csproj", "DoctorAppointmentSystem.Service/"]
COPY ["DoctorAppointmentSystem.Model/DoctorAppointmentSystem.Model.csproj", "DoctorAppointmentSystem.Model/"]
RUN dotnet restore "./DoctorAppointmentSystem.WorkerService/DoctorAppointmentSystem.WorkerService.csproj"
COPY . .
WORKDIR "/src/DoctorAppointmentSystem.WorkerService"
RUN dotnet build "./DoctorAppointmentSystem.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Bu aşama, son aşamaya kopyalanacak hizmet projesini yayımlamak için kullanılır
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DoctorAppointmentSystem.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Bu aşama üretimde veya VS'den normal modda çalıştırıldığında kullanılır (Hata Ayıklama yapılandırması kullanılmazken varsayılan olarak ayarlıdır)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoctorAppointmentSystem.WorkerService.dll"]