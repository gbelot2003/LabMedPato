using System;

namespace FormRender.Models
{
    public abstract class ResponseBase
    {
        public int? id;
        public DateTime?
            created_at,
            updated_at;
    }
    public class InformeResponse:ResponseBase
    {
        public int?
            serial,
            factura_id,
            link_id,
            firma_id,
            firma2_id;

        public string topog;

        // Título?
        public string diagnostico;

        // título de muestra
        public string muestra;

        // Campos de morfología
        public string mor1, mor2;

        // Fechas (parsear a DateTime)
        public DateTime?
            fecha_informe,
            fecha_biopcia,
            fecha_muestra;

        // Información del informe (en html)
        public string informe;

        public bool? muestra_entrega;

        public ImagenResponse[] images;

        public FacturaResponse facturas;

        public FirmaResponse firma;

        public FirmaResponse firma2;
    }
    public class ImagenResponse : ResponseBase,IComparable<ImagenResponse>
    {
        public int? link_id;
        public string image_url;
        public string descripcion;
        public int? order;
        public int CompareTo(ImagenResponse other)
        {
            return order?.CompareTo(other.order) ?? 0;
        }
    }
    public class FacturaResponse: ResponseBase
    {
        public int? num_factura, num_cedula;
        public string
            nombre_completo_cliente,
            fecha_nacimiento,
            edad,
            correo,
            correo2,
            direccion_entrega_sede,
            medico,
            status,
            sexo;
        public float total_factura;
    }
    public class FirmaResponse: ResponseBase
    {
        public string
            name,
            collegiate,
            extra;
        public int? status;
    }
}