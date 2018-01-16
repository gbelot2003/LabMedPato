using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using FormRender.Models;
using TheXDS.MCART;
using HTC = HTMLConverter.HtmlToXamlConverter;
using XA = System.Windows.Markup.XamlReader;

namespace FormRender.Utils
{
    /* -= NOTA =-
     * Esta implementación de interoperatibilidad con Microsoft Word utiliza 
     * argumentos de tipo Object pasados por referencia para todos los métodos
     * y funciones. Esto es muy problemático y engorroso si no se tiene 
     * cuidado. Por lo tanto, las prácticas de acceso podrían parecer
     * incorrectas o no óptimas. Sin embargo, lo mencionado aquí es el motivo.
     */

    public class WordInterop
    {
        readonly Application wordApp = new Application();

        object templPath;

        /// <summary>
        /// Extrae un Template a un archivo temporal.
        /// </summary>
        /// <param name="language"></param>
        /// <returns>La ruta del archivo extraído del ensamblado.</returns>
        public async Task<string> UnpackTemplate(Language language)
        {
            using (var s = typeof(WordInterop).Assembly.GetManifestResourceStream($"FormRender.Assets.{language.ToString()}.dotx"))
            using (var w = new FileStream($"{Path.GetTempFileName()}.dotx", FileMode.Create))
            {
                await s.CopyToAsync(w);
                await w.FlushAsync();
                File.Delete(w.Name.Replace(".dotx", ""));
                return w.Name;
            }
        }
        public async Task<Document> OpenTemplate(Language language)
        {
            templPath = await UnpackTemplate(language);
            return wordApp.Documents.Add(ref templPath);
        }
        public async void Convert(InformeResponse data, IEnumerable<LabeledImage> imgs, Language language)
        {
            var doc = await OpenTemplate(language);


            doc.Variables["Biopsia"].Value = $"{data.serial.ToString() ?? "N/A"} - {data.fecha_biopcia?.Year.ToString() ?? "N/A"}";
            doc.Variables["Diagnostico"].Value = data.diagnostico;
            doc.Variables["Direccion"].Value = data.facturas.direccion_entrega_sede;
            doc.Variables["Doctor"].Value = data.facturas.medico;
            doc.Variables["Edad"].Value = data.facturas.edad;
            doc.Variables["Factura"].Value = $"{data.factura_id.ToString() ?? "N/A"}";
            doc.Variables["Fecha"].Value = $"{data.fecha_biopcia?.ToString("dd/MM/yyyy")}";
            doc.Variables["Material"].Value = data.muestra;
            doc.Variables["Paciente"].Value = data.facturas.nombre_completo_cliente;
            doc.Variables["Recibido"].Value = $"{data.fecha_muestra?.ToString("dd/MM/yyyy")}";
            doc.Variables["Sexo"].Value = data.facturas.sexo;
            UpdateFields(doc);

            string path = Path.GetTempFileName();
            File.WriteAllText(path, $"<html>{data.informe}</html>");
            doc.Content.InsertFile(path, ConfirmConversions: false);
            File.Delete(path);

            doc.Content.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphJustify;

            float lastTop=0;
            foreach (var j in data.images)
            {
                var shp = doc.Shapes.AddPicture($"{Config.imgPath}{j.image_url}", true, true);

                shp.Height = shp.Height * 120 / shp.Width;
                shp.Width = 120;
                shp.AlternativeText = j.descripcion;
                shp.Top = lastTop;
                shp.Left = 400;
                shp.WrapFormat.Type = WdWrapType.wdWrapSquare;
                lastTop += shp.Height;
            }
            wordApp.Visible = true;
        }
        public void UpdateFields(Document doc)
        {
            var pAlerts = wordApp.DisplayAlerts;
            wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            foreach (Range pRange in doc.StoryRanges)
            {
                pRange.Fields.Update();
                switch (pRange.StoryType)
                {
                    case WdStoryType.wdEvenPagesHeaderStory:
                    case WdStoryType.wdPrimaryHeaderStory:
                    case WdStoryType.wdEvenPagesFooterStory:
                    case WdStoryType.wdPrimaryFooterStory:
                    case WdStoryType.wdFirstPageHeaderStory:
                    case WdStoryType.wdFirstPageFooterStory:
                        if (pRange.ShapeRange.Count > 0)
                        {
                            foreach (Shape oShp in pRange.ShapeRange)
                            {
                                if (oShp.TextFrame.HasText != 0)
                                {
                                    oShp.TextFrame.TextRange.Fields.Update();
                                }
                            }
                        }
                        break;
                }
            }
            wordApp.DisplayAlerts = pAlerts;
        }
    }
}