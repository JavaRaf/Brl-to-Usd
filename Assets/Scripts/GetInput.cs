using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class InputHandler : MonoBehaviour
{
    public Button converter; // Variável para o botão
    public TMP_InputField input;
    public TextMeshProUGUI outputText; // Variável para o texto de saída

    void Start()
    {
        converter.onClick.AddListener(async () => await Converter());
    }

    async Task Converter()
    {
        if (float.TryParse(input.text, out float valor))
        {
            string result = await GetValue();
            float bidValue = ExtractBidValue(result);
            if (bidValue != 0)
            {
                float convertedValue = bidValue * valor;
                string convertedValueString = convertedValue.ToString("F2"); // Converte para string com duas casas decimais

                if (valor != 1)
                {
                    string truncatedValue = convertedValueString.Substring(0, Mathf.Min(4, convertedValueString.Length));
                    truncatedValue = truncatedValue.Substring(0,2) + "." +  truncatedValue.Substring(2);
                    outputText.text = truncatedValue;
                } 

                else
                {
                    string truncatedValue = convertedValueString.Substring(0, Mathf.Min(3, convertedValueString.Length));
                    truncatedValue = truncatedValue.Substring(0,1) + "." +  truncatedValue.Substring(1);
                    outputText.text = truncatedValue;

                }
                
            }
            else
            {
                outputText.text = "Erro ao processar o valor de 'bid'.";
            }
        }
        else
        {
            outputText.text = "Por favor, insira um número válido.";
        }
    }

    async Task<string> GetValue()
    {
        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync("https://economia.awesomeapi.com.br/last/USD-BRL");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
            return null;
        }
    }

    float ExtractBidValue(string json)
    {
        try
        {
            JObject jsonObject = JObject.Parse(json);
            string bidValue = jsonObject["USDBRL"]["bid"].ToString();

            if (float.TryParse(bidValue, out float value))
            {
                return value;
            }
            else
            {
                Debug.LogError("Erro ao converter o valor de 'bid' para float.");
                return 0;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON parse error: {e.Message}");
            return 0;
        }
    }
}
