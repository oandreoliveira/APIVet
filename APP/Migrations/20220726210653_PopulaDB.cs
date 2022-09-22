using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiVet.Migrations
{
    public partial class PopulaDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {



            migrationBuilder.Sql(@"INSERT INTO usuarios VALUES 
            (1,'veterinario@gft.com','Gft@1234',0,1),
            (2,'cliente@gft.com','Gft@1234',1,1)");


            migrationBuilder.Sql(@"INSERT INTO veterinarios VALUES 
            (1,'Carlos Alberto',123456,1),
            (2,'Paulo Sérgio Costa',654321,1),
            (3,'Claudio Castro',123789,1),
            (4,'João Filho Silva',789123,1)");



            migrationBuilder.Sql(@"INSERT INTO clientes VALUES 
            (1,'Carlos Costa','11111111111','Rua da Luz, 35, Recife','988888888',1),
            (2,'Breno Silva','22222222222','Rua da Paz, 20, Recife','988888889',1),
            (3,'Diogo Almeida','44444444444','Rua da Aurora, 95, Recife','988888555',1),
            (4,'Arcenio Souzaa','33333333333','Av Santos Dummont, 205, Recife','988888899',1)");

            migrationBuilder.Sql(@"INSERT INTO cachorros VALUES 
            (1,'Biriba','SRD','2022-01-26 19:51:12.251000','Macho',3,1,1),
            (2,'Bob','Pastor Alemão','2021-02-26 19:51:12.251000','Macho',12,1,1),
            (3,'Xuxa','Poodle','2022-02-26 19:51:12.251000','Fêmea',4,2,1),
            (4,'Peteca','Beagle','2020-08-26 19:51:12.251000','Fêmea',10,2,1),
            (5,'Cacau','SRD','2019-08-26 19:51:12.251000','Fêmea',12,3,1),
            (6,'Violento','Pincher','2020-09-26 19:51:12.251000','Macho',2,4,1),
            (7,'Pérola','Labrador','2017-02-26 19:51:12.251000','Fêmea',15,4,1)");


            migrationBuilder.Sql(@"INSERT INTO consultas() VALUES 
            (1,'2022-07-26 17:03:14.481547',4,'Coceira e queda de pêlos','Alergia Alimentar','Antialérgico de 12 em 12 horas por 7 dias',1,1,1,1),
            (2,'2022-07-26 17:04:21.517950',5,'Falta de apetite','Doença dental','Anti-inflamatórios duas vezes ao dia por 1 semana e troca da marca de ração.',2,1,1,1),
            (3,'2022-07-26 17:05:26.327259',6,'Vômitos e diarreias','Parvovirose','Vacina polivalente e antibióticos diários por 15 dias',4,3,2,1),
            (4,'2022-07-26 17:06:26.176893',9,'Vermelhidão, coceira e aumento da secreção do ouvido','Otite','Vacina polivalente e antibióticos diários por 15 dias',4,4,2,1),
            (5,'2022-07-26 17:07:23.874898',9,'Forte diarréia','Cinomose canina','Ribavirina, 30 mg/kg ao dia ',4,5,3,1),
            (6,'2022-07-26 17:11:53.726223',9,'Tosse crônica, fraqueza e respiração acelerada','Dirofilariose','Heparina, diuréticos e terapia de suporte ',3,6,4,1),
            (7,'2022-07-26 17:11:58.726223',9,'Diarreia, perda de peso e anemia','Verme','Ivermectina 3 mg diariamente por 4 dias',2,7,4,1)");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM usuarios");
            migrationBuilder.Sql("DELETE FROM consultas");
            migrationBuilder.Sql("DELETE FROM veterinarios");
            migrationBuilder.Sql("DELETE FROM cachorros");
            migrationBuilder.Sql("DELETE FROM clientes");

        }
    }
}
