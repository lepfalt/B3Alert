# B3 Alert

Esse aplicativo monitora um determinado ativo e notifica via email em momentos propensos para compra e venda.
A entrada esperada é `dotnet run -- <stock> <sell-price> <buy-price>`

## Como foi estruturado

Na pasta /configuration temos as configurações da api de cotação, do email a ser enviado e também o token recebido em caso de autenticações OAuth.

Em /models ficam as classes utilizadas para deserializar as configurações previamente definidas.

Os demais arquivos foram divididos por funcionalidade, sendo assim, toda ação relacionada à envio de email fica em MailAlert, já em QuotationApi fica a lógica de comunicação com a API de cotações e, por fim, em Program fica o método de inicialização da aplicação.

## Como configurar

Inicialmente, é necessário criar o `appsettings.json` com base no `appsettings.example.json` e preencher as credenciais de email e apiKey. 

Para o email, é importante se atentar ao tipo de autenticação, alguns servidores não possuem mais suporte a basicAuth, nesses casos precisamos usar o OAuth. No meu caso, utilizei o google, então foi necessário configurar meu projeto em Google Cloud e habilitar o OAuth (Saiba mais em: [Google OAuth Configuration] {https://developers.google.com/identity/protocols/oauth2}).

Já para API de cotações, utilizei a TwelveData. Foi necessário criar a APIKey e referenciá-la no projeto. (Saiba mais em [TwelveData API]{https://twelvedata.com/docs#websocket}).

Com esses passos já é possível executar a aplicação e passar a receber emails com as sugestões de compra e venda do ativo :D