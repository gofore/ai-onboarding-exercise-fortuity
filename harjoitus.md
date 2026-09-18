# Harjoitus: tuntematon koodikanta

Harjoitus AI-koodausagentin käyttöön tilanteessa, jossa kehittäjä tulee uuteen, dokumentoimattomaan projektiin. Sopii kotitehtäväksi koulutuksen jälkeen tai itsenäiseen harjoitteluun.

---

## Lyhyt versio

**Saat käsiisi projektin, jota et tunne**

- Repo: https://github.com/gofore/ai-onboarding-exercise-fortuity — ei dokumentaatiota
- Tee kaikki AI-agentilla, älä käsin. Pelkkä Docker riittää ajamiseen, .NET:iä ei tarvitse asentaa
- 1. Selvitä: mikä tämä on ja mitä se tekee?
- 2. Käynnistä: saa sovellus pyörimään ja todenna, että se vastaa
- 3. Dokumentoi: README, jossa arkkitehtuurikuva (Mermaid), käynnistysohje ja API:n pääkohdat
- Bonus: tee dokumentaatiostasi pull request, kouluttaja tai tiimikaveri katselmoi
- Pohdi: missä agentti oli hyvä, missä se meni metsään, mitä tarkistit itse?

---

## Pitkä versio

### Tilanne

Sinut on siirretty projektiin, jota et ole nähnyt aiemmin. Edellinen tiimi on lähtenyt eikä jättänyt dokumentaatiota. Tämä on simulaatio tilanteesta, jossa uusi kehittäjä tulee vanhaan projektiin tai projekti siirtyy tiimiltä toiselle.

Repo: https://github.com/gofore/ai-onboarding-exercise-fortuity

Koodi on julkista harjoitusmateriaalia. Kaikki data on keksittyä, eikä repo sisällä minkään organisaation oikeaa tietoa. Sitä saa käsitellä vapaasti millä tahansa AI-työkalulla, jonka käyttö on organisaatiossasi sallittu.

### Tehtävä

Tee koko tehtävä AI-koodausagentilla (esimerkiksi Codex, Claude Code tai vastaava). Tarkoitus ei ole lukea koodia itse ensin, vaan ohjata agenttia ja arvioida sen tuotoksia.

1. **Selvitä, mikä tämä on.** Kloonaa repo ja käynnistä agentti repon juuressa. Pyydä agenttia kertomaan, mitä sovellus tekee, mitkä ovat sen keskeiset käsitteet ja miten se on rakennettu. Kysy jatkokysymyksiä, kunnes ymmärrät itse.

2. **Saa se käyntiin.** Repossa on `compose.yaml`, joten pelkkä Docker riittää. Pyydä agenttia käynnistämään sovellus ja todentamaan, että se vastaa. Hyviä tarkistuspisteitä ovat API:n `/health`-osoite, asiakaslistaus `/api/customers` ja API-dokumentaatio `/scalar`. Selain avautuu osoitteeseen http://localhost:5044, jos portti on käytettävissä agentin ajoympäristön ulkopuolelta. Jos ei ole, riittää, että agentti todentaa vastaukset komentoriviltä.

3. **Dokumentoi.** Pyydä agenttia kirjoittamaan README, joka sisältää ainakin
   - mitä sovellus tekee ja kenelle
   - arkkitehtuurin Mermaid-kaaviona (projektit ja niiden riippuvuudet, tai pyyntöjen kulku)
   - käynnistysohjeen
   - API:n pääkohdat
   - "uuden kehittäjän huomiot": mitä yllättävää, epäselvää tai riskialtista koodista löytyi.

4. **Bonus 1: pull request.** Tee dokumentaatiostasi pull request tähän repoon tai omaan forkkiisi, ja pyydä kouluttajaa tai tiimikaveria katselmoimaan se. Merkitse PR:n kuvaukseen, että se on tehty AI-avusteisesti.

5. **Bonus 2: testi ensin.** Pyydä agenttia etsimään koodista yksi bugi tai epäjohdonmukaisuus ja kirjoittamaan sille ensin epäonnistuva testi, sitten korjaus. Aja testit Dockerin sisällä.

### Muista

- Aloita suunnittelumoodissa. Pyydä suunnitelma ja tarkentavat kysymykset ennen kuin agentti tekee mitään.
- Seuraa kontekstin täyttymistä. Tyhjennä konteksti suunnitelman ja toteutuksen välissä, jos se on yli puolillaan.
- Katselmoi kaikki, mitä agentti kirjoittaa, samalla vaatimustasolla kuin ihmisen tekemä koodi.
- Älä syötä agentille oman organisaatiosi dataa tai koodia tämän harjoituksen yhteydessä. Harjoitus tehdään vain tällä repolla.

### Pohdittavaa

- Missä agentti oli selvästi hyvä? Missä se arvasi tai hallusinoi?
- Kuinka paljon luotit dokumentaatioon tarkistamatta sitä koodista?
- Mitä tekisit toisin, jos koodikanta olisi oman tiimisi oikea koodi?
- Paljonko aikaa kului ja miten se vertautuu siihen, että olisit tehnyt saman käsin?
