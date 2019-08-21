FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY . .

EXPOSE 80
ENV ASPNETCORE_URLS='http://+'
CMD ["DOTNET", "BasicWebAPI.dll"]
