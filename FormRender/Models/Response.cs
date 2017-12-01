using System;

namespace FormRender.Models
{
    public class InformeResponse
    {
        public int?
            id,
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
        public string
            fecha_informe,
            fecha_biopsia,
            fecha_muestra;

        // Información del informe (en html)
        public string informe;

        public string
            created_at,
            updated_at;

        public bool? muestra_entrega;

        public ImagenResponse[] images;

        public FacturaResponse facturas;
    }

    public class ImagenResponse : IComparable<ImagenResponse>
    {
        public int? id, link_id;
        public string image_url;
        public string
            created_at,
            updated_at;
        public string descripcion;
        public int? order;

        public int CompareTo(ImagenResponse other)
        {
            return order?.CompareTo(other.order) ?? 0;
        }
    }

    public class FacturaResponse
    {
        public int? id, num_factura, num_cedula;

        public string
            nombre_completo_cliente,
            fecha_nacimiento,
            edad,
            correo,
            correo2,
            direccion_entrega_sede,
            medico,
            status,
            sexo,
            created_at,
            updated_at;
        public float total_factura;
    }

}