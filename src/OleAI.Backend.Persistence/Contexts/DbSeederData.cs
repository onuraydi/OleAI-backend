namespace OleAI.Backend.Persistence.Contexts
{
    public static class DbSeederData
    {
        public const string IngredientsJson = """
[
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000001",
    "CategoryName": "Sebzeler",
    "Ingredients": [
      { "Name": "Domates", "MeasurementCategory": "Weight" },
      { "Name": "Çeri Domates", "MeasurementCategory": "Weight" },
      { "Name": "Sivri Biber", "MeasurementCategory": "Weight" },
      { "Name": "Çarliston Biber", "MeasurementCategory": "Weight" },
      { "Name": "Dolmalık Biber", "MeasurementCategory": "Weight" },
      { "Name": "Kapya Biber", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Soğan", "MeasurementCategory": "Weight" },
      { "Name": "Arpacık Soğan", "MeasurementCategory": "Weight" },
      { "Name": "Sarımsak", "MeasurementCategory": "Count" },
      { "Name": "Taze Sarımsak", "MeasurementCategory": "Count" },
      { "Name": "Patates", "MeasurementCategory": "Weight" },
      { "Name": "Tatlı Patates", "MeasurementCategory": "Weight" },
      { "Name": "Havuç", "MeasurementCategory": "Weight" },
      { "Name": "Kabak", "MeasurementCategory": "Weight" },
      { "Name": "Bal Kabağı", "MeasurementCategory": "Weight" },
      { "Name": "Kemer Patlıcan", "MeasurementCategory": "Weight" },
      { "Name": "Bostan Patlıcan", "MeasurementCategory": "Weight" },
      { "Name": "Ispanak", "MeasurementCategory": "Weight" },
      { "Name": "Brokoli", "MeasurementCategory": "Weight" },
      { "Name": "Karnabahar", "MeasurementCategory": "Weight" },
      { "Name": "Pırasa", "MeasurementCategory": "Weight" },
      { "Name": "Kereviz", "MeasurementCategory": "Weight" },
      { "Name": "Kereviz Sapı", "MeasurementCategory": "Weight" },
      { "Name": "Taze Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Ayşe Kadın Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Çalı Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Taze Barbunya", "MeasurementCategory": "Weight" },
      { "Name": "Taze Bakla", "MeasurementCategory": "Weight" },
      { "Name": "Taze Börülce (Deniz Börülcesi)", "MeasurementCategory": "Weight" },
      { "Name": "Bezelye", "MeasurementCategory": "Weight" },
      { "Name": "Enginar", "MeasurementCategory": "Count" },
      { "Name": "Kuşkonmaz", "MeasurementCategory": "Weight" },
      { "Name": "Bamya", "MeasurementCategory": "Weight" },
      { "Name": "Pazı", "MeasurementCategory": "Weight" },
      { "Name": "Semizotu", "MeasurementCategory": "Weight" },
      { "Name": "Karalahana", "MeasurementCategory": "Weight" },
      { "Name": "Beyaz Lahana", "MeasurementCategory": "Weight" },
      { "Name": "Kırmızı Lahana", "MeasurementCategory": "Weight" },
      { "Name": "Brüksel Lahanası", "MeasurementCategory": "Weight" },
      { "Name": "Marul", "MeasurementCategory": "Count" },
      { "Name": "Kıvırcık Marul", "MeasurementCategory": "Count" },
      { "Name": "Göbek Marul (Iceberg)", "MeasurementCategory": "Count" },
      { "Name": "Roka", "MeasurementCategory": "Weight" },
      { "Name": "Tere", "MeasurementCategory": "Weight" },
      { "Name": "Taze Soğan", "MeasurementCategory": "Weight" },
      { "Name": "Maydanoz", "MeasurementCategory": "Weight" },
      { "Name": "Dereotu", "MeasurementCategory": "Weight" },
      { "Name": "Nane (Taze)", "MeasurementCategory": "Weight" },
      { "Name": "Kırmızı Turp", "MeasurementCategory": "Weight" },
      { "Name": "Siyah Turp", "MeasurementCategory": "Weight" },
      { "Name": "Beyaz Turp", "MeasurementCategory": "Weight" },
      { "Name": "Alabaş Turpu", "MeasurementCategory": "Weight" },
      { "Name": "Şalgam", "MeasurementCategory": "Weight" },
      { "Name": "Pancar", "MeasurementCategory": "Weight" },
      { "Name": "Şevketi Bostan", "MeasurementCategory": "Weight" },
      { "Name": "Cibes Otu", "MeasurementCategory": "Weight" },
      { "Name": "Radika (Karahindiba)", "MeasurementCategory": "Weight" },
      { "Name": "Isırgan Otu", "MeasurementCategory": "Weight" },
      { "Name": "Ebegümeci", "MeasurementCategory": "Weight" },
      { "Name": "Madımak", "MeasurementCategory": "Weight" },
      { "Name": "Çiriş Otu", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000021",
    "CategoryName": "Mantarlar",
    "Ingredients": [
      { "Name": "Kültür Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "İstiridye Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Kestane Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Porçini Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Kanlıca Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Göbeği Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Çıntar Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Şitaki Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Trüf Mantarı", "MeasurementCategory": "Weight" },
      { "Name": "Enoki Mantarı", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000002",
    "CategoryName": "Meyveler",
    "Ingredients": [
      { "Name": "Amasya Elması", "MeasurementCategory": "Count" },
      { "Name": "Granny Smith Elma", "MeasurementCategory": "Count" },
      { "Name": "Muz", "MeasurementCategory": "Count" },
      { "Name": "Portakal", "MeasurementCategory": "Count" },
      { "Name": "Limon", "MeasurementCategory": "Count" },
      { "Name": "Çilek", "MeasurementCategory": "Weight" },
      { "Name": "Siyah Üzüm", "MeasurementCategory": "Weight" },
      { "Name": "Çekirdeksiz Beyaz Üzüm", "MeasurementCategory": "Weight" },
      { "Name": "Diyarbakır Karpuzu", "MeasurementCategory": "Weight" },
      { "Name": "Kırkağaç Kavunu", "MeasurementCategory": "Weight" },
      { "Name": "Ankara Armudu", "MeasurementCategory": "Count" },
      { "Name": "Deveci Armudu", "MeasurementCategory": "Count" },
      { "Name": "Avokado", "MeasurementCategory": "Count" },
      { "Name": "Bodrum Mandalinası", "MeasurementCategory": "Count" },
      { "Name": "Bursa Şeftalisi", "MeasurementCategory": "Count" },
      { "Name": "Nektarin", "MeasurementCategory": "Count" },
      { "Name": "Malatya Kayısısı", "MeasurementCategory": "Count" },
      { "Name": "Napolyon Kirazı", "MeasurementCategory": "Weight" },
      { "Name": "Vişne", "MeasurementCategory": "Weight" },
      { "Name": "Mürdüm Eriği", "MeasurementCategory": "Weight" },
      { "Name": "Yeşil Erik (Can Erik)", "MeasurementCategory": "Weight" },
      { "Name": "Aydın İnciri", "MeasurementCategory": "Count" },
      { "Name": "Siyah İncir", "MeasurementCategory": "Count" },
      { "Name": "Nar", "MeasurementCategory": "Count" },
      { "Name": "Kivi", "MeasurementCategory": "Count" },
      { "Name": "Ananas", "MeasurementCategory": "Count" },
      { "Name": "Mango", "MeasurementCategory": "Count" },
      { "Name": "Ahududu", "MeasurementCategory": "Weight" },
      { "Name": "Böğürtlen", "MeasurementCategory": "Weight" },
      { "Name": "Yaban Mersini", "MeasurementCategory": "Weight" },
      { "Name": "Karadut", "MeasurementCategory": "Weight" },
      { "Name": "Kızılcık", "MeasurementCategory": "Weight" },
      { "Name": "Eşme Ayvası", "MeasurementCategory": "Count" },
      { "Name": "Greyfurt", "MeasurementCategory": "Count" },
      { "Name": "Muşmula", "MeasurementCategory": "Weight" },
      { "Name": "İğde", "MeasurementCategory": "Weight" },
      { "Name": "Hünnap", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000003",
    "CategoryName": "Kırmızı Et",
    "Ingredients": [
      { "Name": "Dana Kıyma (Yağlı)", "MeasurementCategory": "Weight" },
      { "Name": "Dana Kıyma (Az Yağlı)", "MeasurementCategory": "Weight" },
      { "Name": "Dana Kuşbaşı", "MeasurementCategory": "Weight" },
      { "Name": "Dana Biftek", "MeasurementCategory": "Weight" },
      { "Name": "Dana Antrikot", "MeasurementCategory": "Weight" },
      { "Name": "Dana Bonfile", "MeasurementCategory": "Weight" },
      { "Name": "Dana Kontrfile", "MeasurementCategory": "Weight" },
      { "Name": "Dana Tranç", "MeasurementCategory": "Weight" },
      { "Name": "Dana Nuar", "MeasurementCategory": "Weight" },
      { "Name": "Dana Döş", "MeasurementCategory": "Weight" },
      { "Name": "Dana Kaburga", "MeasurementCategory": "Weight" },
      { "Name": "Dana İncik", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Kıyma", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Kuşbaşı", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Pirzola", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu İncik", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Gerdan", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Kaburga", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Kol", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu But", "MeasurementCategory": "Weight" },
      { "Name": "Kuyruk Yağı", "MeasurementCategory": "Weight" },
      { "Name": "İç Yağı", "MeasurementCategory": "Weight" },
      { "Name": "Dana Ciğer", "MeasurementCategory": "Weight" },
      { "Name": "Kuzu Ciğer", "MeasurementCategory": "Weight" },
      { "Name": "Yürek", "MeasurementCategory": "Weight" },
      { "Name": "Böbrek", "MeasurementCategory": "Weight" },
      { "Name": "İşkembe", "MeasurementCategory": "Weight" },
      { "Name": "Şırdan", "MeasurementCategory": "Count" },
      { "Name": "Mumbar", "MeasurementCategory": "Weight" },
      { "Name": "Kelle Eti", "MeasurementCategory": "Weight" },
      { "Name": "Paça", "MeasurementCategory": "Count" },
      { "Name": "Dil", "MeasurementCategory": "Weight" },
      { "Name": "Beyin", "MeasurementCategory": "Count" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000022",
    "CategoryName": "Kümes Hayvanları",
    "Ingredients": [
      { "Name": "Bütün Tavuk", "MeasurementCategory": "Count" },
      { "Name": "Köy Tavuğu", "MeasurementCategory": "Count" },
      { "Name": "Tavuk Göğsü", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk Baget", "MeasurementCategory": "Count" },
      { "Name": "Tavuk Kanat", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk İncik", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk Pirzola", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk But", "MeasurementCategory": "Count" },
      { "Name": "Tavuk Kıyma", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk Ciğeri", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk Taşlık", "MeasurementCategory": "Weight" },
      { "Name": "Bütün Hindi", "MeasurementCategory": "Count" },
      { "Name": "Hindi Göğsü", "MeasurementCategory": "Weight" },
      { "Name": "Hindi Baget", "MeasurementCategory": "Count" },
      { "Name": "Ördek Eti", "MeasurementCategory": "Weight" },
      { "Name": "Kaz Eti", "MeasurementCategory": "Weight" },
      { "Name": "Bıldırcın Eti", "MeasurementCategory": "Count" },
      { "Name": "Keklik Eti", "MeasurementCategory": "Count" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000023",
    "CategoryName": "Şarküteri ve İşlenmiş Etler",
    "Ingredients": [
      { "Name": "Kangal Sucuk (Kasap)", "MeasurementCategory": "Weight" },
      { "Name": "Parmak Sucuk", "MeasurementCategory": "Weight" },
      { "Name": "Sucuk İçi", "MeasurementCategory": "Weight" },
      { "Name": "Sosis", "MeasurementCategory": "Count" },
      { "Name": "Salam", "MeasurementCategory": "Weight" },
      { "Name": "Kayseri Pastırması (Sırt)", "MeasurementCategory": "Weight" },
      { "Name": "Kastamonu Pastırması", "MeasurementCategory": "Weight" },
      { "Name": "Füme Hindi", "MeasurementCategory": "Weight" },
      { "Name": "Dana Jambon", "MeasurementCategory": "Weight" },
      { "Name": "Tavuk Jambon", "MeasurementCategory": "Weight" },
      { "Name": "Kavurma (Rize)", "MeasurementCategory": "Weight" },
      { "Name": "Kavurma (Klasik)", "MeasurementCategory": "Weight" },
      { "Name": "Rozbif", "MeasurementCategory": "Weight" },
      { "Name": "Pepperoni", "MeasurementCategory": "Weight" },
      { "Name": "Çemen", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000004",
    "CategoryName": "Deniz Ürünleri",
    "Ingredients": [
      { "Name": "Somon Filato", "MeasurementCategory": "Weight" },
      { "Name": "Deniz Çipurası", "MeasurementCategory": "Count" },
      { "Name": "Kültür Çipurası", "MeasurementCategory": "Count" },
      { "Name": "Deniz Levreği", "MeasurementCategory": "Count" },
      { "Name": "Karadeniz Hamsisi", "MeasurementCategory": "Weight" },
      { "Name": "İstavrit", "MeasurementCategory": "Weight" },
      { "Name": "Lüfer", "MeasurementCategory": "Count" },
      { "Name": "Sarıkanat", "MeasurementCategory": "Count" },
      { "Name": "Çinekop", "MeasurementCategory": "Weight" },
      { "Name": "Palamut", "MeasurementCategory": "Count" },
      { "Name": "Mezgit", "MeasurementCategory": "Weight" },
      { "Name": "Tekir", "MeasurementCategory": "Weight" },
      { "Name": "Barbunya Balığı", "MeasurementCategory": "Weight" },
      { "Name": "Kalkan Balığı", "MeasurementCategory": "Weight" },
      { "Name": "Sardalya", "MeasurementCategory": "Weight" },
      { "Name": "Uskumru", "MeasurementCategory": "Count" },
      { "Name": "Kılıç Balığı", "MeasurementCategory": "Weight" },
      { "Name": "Karides (Çimçim)", "MeasurementCategory": "Weight" },
      { "Name": "Jumbo Karides", "MeasurementCategory": "Count" },
      { "Name": "Kalamar", "MeasurementCategory": "Weight" },
      { "Name": "Midye (İç)", "MeasurementCategory": "Weight" },
      { "Name": "Kabuklu Midye", "MeasurementCategory": "Count" },
      { "Name": "Ahtapot", "MeasurementCategory": "Weight" },
      { "Name": "Yengeç", "MeasurementCategory": "Weight" },
      { "Name": "Istakoz", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000005",
    "CategoryName": "Süt ve Süt Ürünleri",
    "Ingredients": [
      { "Name": "Tam Yağlı İnek Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Yarım Yağlı İnek Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Keçi Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Manda Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Koyun Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Köy Yoğurdu", "MeasurementCategory": "Weight" },
      { "Name": "Süzme Yoğurt", "MeasurementCategory": "Weight" },
      { "Name": "Manda Yoğurdu", "MeasurementCategory": "Weight" },
      { "Name": "Kurut (Kurutulmuş Yoğurt)", "MeasurementCategory": "Weight" },
      { "Name": "Ezine Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Klasik Beyaz Peynir", "MeasurementCategory": "Weight" },
      { "Name": "Taze Kaşar Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Eski Kaşar Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Kars Gravyeri", "MeasurementCategory": "Weight" },
      { "Name": "Erzincan Tulum Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "İzmir Tulum Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Van Otlu Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Diyarbakır Örgü Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Erzurum Çeçil Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Mihaliç Peyniri (Kelle)", "MeasurementCategory": "Weight" },
      { "Name": "Divle Obruk Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Kolot Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Küp Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Lor Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Çökelek", "MeasurementCategory": "Weight" },
      { "Name": "Labne Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Mozzarella Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Cheddar Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Parmesan Peyniri", "MeasurementCategory": "Weight" },
      { "Name": "Krem Peynir", "MeasurementCategory": "Weight" },
      { "Name": "Trabzon Tereyağı", "MeasurementCategory": "Weight" },
      { "Name": "Tuzsuz Tereyağı", "MeasurementCategory": "Weight" },
      { "Name": "Süt Kreması", "MeasurementCategory": "Volume" },
      { "Name": "Afyon Kaymağı", "MeasurementCategory": "Weight" },
      { "Name": "Kefir", "MeasurementCategory": "Volume" },
      { "Name": "Kımız", "MeasurementCategory": "Volume" },
      { "Name": "Tavuk Yumurtası", "MeasurementCategory": "Count" },
      { "Name": "Bıldırcın Yumurtası", "MeasurementCategory": "Count" },
      { "Name": "Ördek Yumurtası", "MeasurementCategory": "Count" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000024",
    "CategoryName": "Bitkisel Sütler ve Alternatifler",
    "Ingredients": [
      { "Name": "Badem Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Yulaf Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Soya Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Hindistan Cevizi Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Pirinç Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Fındık Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Kaju Sütü", "MeasurementCategory": "Volume" },
      { "Name": "Tofu", "MeasurementCategory": "Weight" },
      { "Name": "Vegan Peynir", "MeasurementCategory": "Weight" },
      { "Name": "Vegan Tereyağı", "MeasurementCategory": "Weight" },
      { "Name": "Soya Kıyması", "MeasurementCategory": "Weight" },
      { "Name": "Soya Eti", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000006",
    "CategoryName": "Tahıllar ve Unlar",
    "Ingredients": [
      { "Name": "Buğday Unu", "MeasurementCategory": "Weight" },
      { "Name": "Tam Buğday Unu", "MeasurementCategory": "Weight" },
      { "Name": "Siyez Unu", "MeasurementCategory": "Weight" },
      { "Name": "Karakılçık Unu", "MeasurementCategory": "Weight" },
      { "Name": "Çavdar Unu", "MeasurementCategory": "Weight" },
      { "Name": "Karadeniz Mısır Unu", "MeasurementCategory": "Weight" },
      { "Name": "Yulaf Unu", "MeasurementCategory": "Weight" },
      { "Name": "Pirinç Unu", "MeasurementCategory": "Weight" },
      { "Name": "Nohut Unu", "MeasurementCategory": "Weight" },
      { "Name": "Karabuğday Unu", "MeasurementCategory": "Weight" },
      { "Name": "Baldo Pirinç", "MeasurementCategory": "Weight" },
      { "Name": "Osmancık Pirinç", "MeasurementCategory": "Weight" },
      { "Name": "Basmati Pirinç", "MeasurementCategory": "Weight" },
      { "Name": "Yasemin Pirinç", "MeasurementCategory": "Weight" },
      { "Name": "Kırık Pirinç", "MeasurementCategory": "Weight" },
      { "Name": "İri Pilavlık Bulgur", "MeasurementCategory": "Weight" },
      { "Name": "Köftelik İnce Bulgur (Düğürcük)", "MeasurementCategory": "Weight" },
      { "Name": "Esmer Bulgur", "MeasurementCategory": "Weight" },
      { "Name": "Firik Bulguru", "MeasurementCategory": "Weight" },
      { "Name": "Siyez Bulguru", "MeasurementCategory": "Weight" },
      { "Name": "Aşurelik Buğday (Dövme/Yarma)", "MeasurementCategory": "Weight" },
      { "Name": "Yulaf Ezmesi", "MeasurementCategory": "Weight" },
      { "Name": "Kinoa", "MeasurementCategory": "Weight" },
      { "Name": "Chia Tohumu", "MeasurementCategory": "Weight" },
      { "Name": "Karabuğday (Greçka)", "MeasurementCategory": "Weight" },
      { "Name": "Keten Tohumu", "MeasurementCategory": "Weight" },
      { "Name": "İrmik", "MeasurementCategory": "Weight" },
      { "Name": "Mısır Nişastası", "MeasurementCategory": "Weight" },
      { "Name": "Buğday Nişastası", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000025",
    "CategoryName": "Bakliyatlar",
    "Ingredients": [
      { "Name": "Kırmızı Mercimek", "MeasurementCategory": "Weight" },
      { "Name": "Yeşil Mercimek", "MeasurementCategory": "Weight" },
      { "Name": "Sarı Mercimek", "MeasurementCategory": "Weight" },
      { "Name": "Koçbaşı Nohut", "MeasurementCategory": "Weight" },
      { "Name": "Leblebilik Nohut", "MeasurementCategory": "Weight" },
      { "Name": "Dermason Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "İspir Fasulyesi", "MeasurementCategory": "Weight" },
      { "Name": "Sıra Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Göğceli Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Barbunya", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Börülce (Karnıkara)", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Bakla", "MeasurementCategory": "Weight" },
      { "Name": "Maş Fasulyesi", "MeasurementCategory": "Weight" },
      { "Name": "Soya Fasulyesi", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000007",
    "CategoryName": "Kuruyemişler ve Tohumlar",
    "Ingredients": [
      { "Name": "Ceviz İçi", "MeasurementCategory": "Weight" },
      { "Name": "Giresun Fındığı", "MeasurementCategory": "Weight" },
      { "Name": "Kavrulmuş Fındık", "MeasurementCategory": "Weight" },
      { "Name": "Kavrulmuş Badem", "MeasurementCategory": "Weight" },
      { "Name": "Çiğ Badem", "MeasurementCategory": "Weight" },
      { "Name": "Kavrulmuş Kaju", "MeasurementCategory": "Weight" },
      { "Name": "Çiğ Kaju", "MeasurementCategory": "Weight" },
      { "Name": "Boz İç Antep Fıstığı", "MeasurementCategory": "Weight" },
      { "Name": "Kavrulmuş Antep Fıstığı", "MeasurementCategory": "Weight" },
      { "Name": "Osmaniye Yer Fıstığı", "MeasurementCategory": "Weight" },
      { "Name": "Çam Fıstığı (Dolmalık Fıstık)", "MeasurementCategory": "Weight" },
      { "Name": "Ay Çekirdeği İçi", "MeasurementCategory": "Weight" },
      { "Name": "Kabak Çekirdeği İçi", "MeasurementCategory": "Weight" },
      { "Name": "Susam", "MeasurementCategory": "Weight" },
      { "Name": "Çörek Otu", "MeasurementCategory": "Weight" },
      { "Name": "Haşhaş Tohumu (Mavi)", "MeasurementCategory": "Weight" },
      { "Name": "Haşhaş Tohumu (Sarı)", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Üzüm (Sultani)", "MeasurementCategory": "Weight" },
      { "Name": "Besni Üzümü", "MeasurementCategory": "Weight" },
      { "Name": "Kuş Üzümü", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Kayısı (Günkurusu)", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Kayısı (Sarı)", "MeasurementCategory": "Weight" },
      { "Name": "Aydın Kuru İnciri", "MeasurementCategory": "Weight" },
      { "Name": "Kudüs Hurması", "MeasurementCategory": "Weight" },
      { "Name": "Medine Hurması", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Erik", "MeasurementCategory": "Weight" },
      { "Name": "Yaban Mersini Kurusu", "MeasurementCategory": "Weight" },
      { "Name": "Kurutulmuş Hindistan Cevizi (Rende)", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000008",
    "CategoryName": "Baharatlar ve Taze Otlar",
    "Ingredients": [
      { "Name": "İyotlu Tuz", "MeasurementCategory": "Weight" },
      { "Name": "Kaya Tuzu", "MeasurementCategory": "Weight" },
      { "Name": "Deniz Tuzu", "MeasurementCategory": "Weight" },
      { "Name": "Karabiber Toz", "MeasurementCategory": "Weight" },
      { "Name": "Tane Karabiber", "MeasurementCategory": "Weight" },
      { "Name": "Kırmızı Toz Biber (Tatlı)", "MeasurementCategory": "Weight" },
      { "Name": "Kırmızı Toz Biber (Acı)", "MeasurementCategory": "Weight" },
      { "Name": "İpek Pul Biber", "MeasurementCategory": "Weight" },
      { "Name": "Şanlıurfa İsotu", "MeasurementCategory": "Weight" },
      { "Name": "Maraş Biberi", "MeasurementCategory": "Weight" },
      { "Name": "Sumak", "MeasurementCategory": "Weight" },
      { "Name": "Kimyon", "MeasurementCategory": "Weight" },
      { "Name": "Bilye Kekik", "MeasurementCategory": "Weight" },
      { "Name": "Zahter (Kuru)", "MeasurementCategory": "Weight" },
      { "Name": "Taze Kekik", "MeasurementCategory": "Weight" },
      { "Name": "Kuru Nane", "MeasurementCategory": "Weight" },
      { "Name": "Fesleğen (Kuru)", "MeasurementCategory": "Weight" },
      { "Name": "Reyhan (Kuru)", "MeasurementCategory": "Weight" },
      { "Name": "Zerdeçal", "MeasurementCategory": "Weight" },
      { "Name": "Zencefil (Toz)", "MeasurementCategory": "Weight" },
      { "Name": "Taze Zencefil", "MeasurementCategory": "Weight" },
      { "Name": "Tarçın (Toz)", "MeasurementCategory": "Weight" },
      { "Name": "Çubuk Tarçın", "MeasurementCategory": "Count" },
      { "Name": "Karanfil", "MeasurementCategory": "Weight" },
      { "Name": "Yenibahar", "MeasurementCategory": "Weight" },
      { "Name": "Kişniş (Toz)", "MeasurementCategory": "Weight" },
      { "Name": "Tane Kişniş", "MeasurementCategory": "Weight" },
      { "Name": "Taze Kişniş", "MeasurementCategory": "Weight" },
      { "Name": "Köri", "MeasurementCategory": "Weight" },
      { "Name": "Biberiye (Kuru)", "MeasurementCategory": "Weight" },
      { "Name": "Taze Biberiye", "MeasurementCategory": "Weight" },
      { "Name": "Defne Yaprağı", "MeasurementCategory": "Count" },
      { "Name": "Safran", "MeasurementCategory": "Weight" },
      { "Name": "Haspir (Yalancı Safran)", "MeasurementCategory": "Weight" },
      { "Name": "Mahlep", "MeasurementCategory": "Weight" },
      { "Name": "Çemen Tozu", "MeasurementCategory": "Weight" },
      { "Name": "Anason", "MeasurementCategory": "Weight" },
      { "Name": "Rezene Tohumu", "MeasurementCategory": "Weight" },
      { "Name": "Havlıcan", "MeasurementCategory": "Weight" },
      { "Name": "Tarhun", "MeasurementCategory": "Weight" },
      { "Name": "Muskat", "MeasurementCategory": "Count" },
      { "Name": "Damla Sakızı", "MeasurementCategory": "Weight" },
      { "Name": "Kakule", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000009",
    "CategoryName": "Sıvı ve Katı Yağlar",
    "Ingredients": [
      { "Name": "Sızma Zeytinyağı", "MeasurementCategory": "Volume" },
      { "Name": "Riviera Zeytinyağı", "MeasurementCategory": "Volume" },
      { "Name": "Ayçiçek Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Mısır Özü Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Fındık Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Kanola Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Susam Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Hindistan Cevizi Yağı", "MeasurementCategory": "Weight" },
      { "Name": "Margarin", "MeasurementCategory": "Weight" },
      { "Name": "Kuyruk Yağı", "MeasurementCategory": "Weight" },
      { "Name": "Sade Yağ (Ghee)", "MeasurementCategory": "Weight" },
      { "Name": "İç Yağı", "MeasurementCategory": "Weight" },
      { "Name": "Avokado Yağı", "MeasurementCategory": "Volume" },
      { "Name": "Ceviz Yağı", "MeasurementCategory": "Volume" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000010",
    "CategoryName": "Soslar, Salçalar ve Çeşniler",
    "Ingredients": [
      { "Name": "Ev Tipi Domates Salçası", "MeasurementCategory": "Weight" },
      { "Name": "Ev Tipi Biber Salçası (Tatlı)", "MeasurementCategory": "Weight" },
      { "Name": "Ev Tipi Biber Salçası (Acı)", "MeasurementCategory": "Weight" },
      { "Name": "Domates Püresi", "MeasurementCategory": "Volume" },
      { "Name": "Mayonez", "MeasurementCategory": "Weight" },
      { "Name": "Ketçap", "MeasurementCategory": "Weight" },
      { "Name": "Soya Sosu", "MeasurementCategory": "Volume" },
      { "Name": "Hardal", "MeasurementCategory": "Weight" },
      { "Name": "100% Nar Ekşisi", "MeasurementCategory": "Volume" },
      { "Name": "Nar Ekşili Sos", "MeasurementCategory": "Volume" },
      { "Name": "Sumak Ekşisi", "MeasurementCategory": "Volume" },
      { "Name": "Elma Sirkesi", "MeasurementCategory": "Volume" },
      { "Name": "Üzüm Sirkesi", "MeasurementCategory": "Volume" },
      { "Name": "Alıç Sirkesi", "MeasurementCategory": "Volume" },
      { "Name": "Balzamik Sirke", "MeasurementCategory": "Volume" },
      { "Name": "Limon Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Barbekü Sos", "MeasurementCategory": "Weight" },
      { "Name": "Acı Sos (Hot Sauce)", "MeasurementCategory": "Volume" },
      { "Name": "Pesto Sos", "MeasurementCategory": "Weight" },
      { "Name": "Tahin (Çifte Kavrulmuş)", "MeasurementCategory": "Weight" },
      { "Name": "Bozkır Tahini", "MeasurementCategory": "Weight" },
      { "Name": "Sriracha", "MeasurementCategory": "Weight" },
      { "Name": "Worcestershire Sosu", "MeasurementCategory": "Volume" },
      { "Name": "Teriyaki Sos", "MeasurementCategory": "Volume" },
      { "Name": "Sweet Chili Sos", "MeasurementCategory": "Weight" },
      { "Name": "Humus (Hazır)", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000026",
    "CategoryName": "Tatlandırıcılar ve Şuruplar",
    "Ingredients": [
      { "Name": "Toz Şeker", "MeasurementCategory": "Weight" },
      { "Name": "Esmer Şeker", "MeasurementCategory": "Weight" },
      { "Name": "Küp Şeker", "MeasurementCategory": "Count" },
      { "Name": "Pudra Şekeri", "MeasurementCategory": "Weight" },
      { "Name": "Çiçek Balı", "MeasurementCategory": "Weight" },
      { "Name": "Çam Balı", "MeasurementCategory": "Weight" },
      { "Name": "Kestane Balı", "MeasurementCategory": "Weight" },
      { "Name": "Karakovan Balı", "MeasurementCategory": "Weight" },
      { "Name": "Üzüm Pekmezi", "MeasurementCategory": "Weight" },
      { "Name": "Dut Pekmezi", "MeasurementCategory": "Weight" },
      { "Name": "Keçiboynuzu Özü (Harnup)", "MeasurementCategory": "Weight" },
      { "Name": "Andız Pekmezi", "MeasurementCategory": "Weight" },
      { "Name": "Karadut Özü", "MeasurementCategory": "Weight" },
      { "Name": "Petimez", "MeasurementCategory": "Weight" },
      { "Name": "Akçaağaç Şurubu (Maple)", "MeasurementCategory": "Volume" },
      { "Name": "Agave Şurubu", "MeasurementCategory": "Volume" },
      { "Name": "Stevia", "MeasurementCategory": "Weight" },
      { "Name": "Vanilya Çubuğu", "MeasurementCategory": "Count" },
      { "Name": "Şekerli Vanilin", "MeasurementCategory": "Weight" },
      { "Name": "Kabartma Tozu", "MeasurementCategory": "Weight" },
      { "Name": "Karbonat", "MeasurementCategory": "Weight" },
      { "Name": "Limon Tuzu", "MeasurementCategory": "Weight" },
      { "Name": "Salep (Toz)", "MeasurementCategory": "Weight" },
      { "Name": "Tahin Helvası", "MeasurementCategory": "Weight" },
      { "Name": "İrmik Helvası", "MeasurementCategory": "Weight" },
      { "Name": "Pişmaniye", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000011",
    "CategoryName": "Unlu Mamuller",
    "Ingredients": [
      { "Name": "Somun Ekmek (Taş Fırın)", "MeasurementCategory": "Count" },
      { "Name": "Tam Buğday Ekmeği", "MeasurementCategory": "Count" },
      { "Name": "Kepekli Ekmek", "MeasurementCategory": "Count" },
      { "Name": "Çavdar Ekmeği", "MeasurementCategory": "Count" },
      { "Name": "Ekşi Mayalı Köy Ekmeği", "MeasurementCategory": "Weight" },
      { "Name": "Lavaş", "MeasurementCategory": "Count" },
      { "Name": "Tırnak Pidesi", "MeasurementCategory": "Count" },
      { "Name": "Ramazan Pidesi", "MeasurementCategory": "Count" },
      { "Name": "Bazlama", "MeasurementCategory": "Count" },
      { "Name": "Gözleme Hamuru", "MeasurementCategory": "Weight" },
      { "Name": "Günlük Yufka", "MeasurementCategory": "Count" },
      { "Name": "Baklavalık Yufka", "MeasurementCategory": "Weight" },
      { "Name": "Milföy Hamuru", "MeasurementCategory": "Count" },
      { "Name": "Mantı Hamuru", "MeasurementCategory": "Weight" },
      { "Name": "Erişte", "MeasurementCategory": "Weight" },
      { "Name": "Kuskus (Türk Usulü)", "MeasurementCategory": "Weight" },
      { "Name": "Tarhana (Toz)", "MeasurementCategory": "Weight" },
      { "Name": "Tel Kadayıf", "MeasurementCategory": "Weight" },
      { "Name": "Güllaç Yaprağı", "MeasurementCategory": "Weight" },
      { "Name": "Kemalpaşa Tatlısı (Kuru)", "MeasurementCategory": "Count" },
      { "Name": "Sandviç Ekmeği", "MeasurementCategory": "Count" },
      { "Name": "Hamburger Ekmeği", "MeasurementCategory": "Count" },
      { "Name": "Simit", "MeasurementCategory": "Count" },
      { "Name": "Açma", "MeasurementCategory": "Count" },
      { "Name": "Poğaça", "MeasurementCategory": "Count" },
      { "Name": "İzmir Boyozu", "MeasurementCategory": "Count" },
      { "Name": "Galeta Unu", "MeasurementCategory": "Weight" },
      { "Name": "Panko", "MeasurementCategory": "Weight" },
      { "Name": "Tortilla Ekmeği", "MeasurementCategory": "Count" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000027",
    "CategoryName": "Çikolata ve Kakao",
    "Ingredients": [
      { "Name": "Bitter Çikolata (%70 Kakao)", "MeasurementCategory": "Weight" },
      { "Name": "Bitter Çikolata (%80+ Kakao)", "MeasurementCategory": "Weight" },
      { "Name": "Sütlü Çikolata", "MeasurementCategory": "Weight" },
      { "Name": "Beyaz Çikolata", "MeasurementCategory": "Weight" },
      { "Name": "Damla Çikolata (Bitter)", "MeasurementCategory": "Weight" },
      { "Name": "Damla Çikolata (Sütlü)", "MeasurementCategory": "Weight" },
      { "Name": "Kakao Tozu", "MeasurementCategory": "Weight" },
      { "Name": "Kuvertür Çikolata", "MeasurementCategory": "Weight" },
      { "Name": "Sürülebilir Çikolata Kremsı (Kakaolu Fındık Kreması)", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000028",
    "CategoryName": "Hazır ve Konserve Gıdalar",
    "Ingredients": [
      { "Name": "Konserve Haşlanmış Mısır", "MeasurementCategory": "Weight" },
      { "Name": "Konserve Ton Balığı", "MeasurementCategory": "Weight" },
      { "Name": "Konserve Bezelye", "MeasurementCategory": "Weight" },
      { "Name": "Konserve Nohut", "MeasurementCategory": "Weight" },
      { "Name": "Konserve Kuru Fasulye", "MeasurementCategory": "Weight" },
      { "Name": "Konserve Barbunya", "MeasurementCategory": "Weight" },
      { "Name": "Közlenmiş Kırmızı Biber (Kapya)", "MeasurementCategory": "Weight" },
      { "Name": "Közlenmiş Patlıcan", "MeasurementCategory": "Weight" },
      { "Name": "Garnitür Konservesi", "MeasurementCategory": "Weight" },
      { "Name": "Siyah Zeytin (Gemlik)", "MeasurementCategory": "Weight" },
      { "Name": "Siyah Zeytin (Sele)", "MeasurementCategory": "Weight" },
      { "Name": "Çizik Yeşil Zeytin", "MeasurementCategory": "Weight" },
      { "Name": "Kırma Yeşil Zeytin", "MeasurementCategory": "Weight" },
      { "Name": "Biber Dolgulu Yeşil Zeytin", "MeasurementCategory": "Weight" },
      { "Name": "Kornişon Turşu", "MeasurementCategory": "Weight" },
      { "Name": "Acur Turşusu", "MeasurementCategory": "Weight" },
      { "Name": "Jalapeno Turşu", "MeasurementCategory": "Weight" },
      { "Name": "Lahana Turşusu", "MeasurementCategory": "Weight" },
      { "Name": "Pancar Turşusu", "MeasurementCategory": "Weight" },
      { "Name": "Kapari", "MeasurementCategory": "Weight" },
      { "Name": "Kurutulmuş Domates", "MeasurementCategory": "Weight" },
      { "Name": "Salamura Asma Yaprağı", "MeasurementCategory": "Weight" },
      { "Name": "Biber Salçası (Hazır)", "MeasurementCategory": "Weight" }
    ]
  },
  {
    "CategoryId": "C0000000-0000-0000-0000-000000000012",
    "CategoryName": "İçecekler",
    "Ingredients": [
      { "Name": "İçme Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Doğal Maden Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Meyveli Maden Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Şalgam Suyu (Acılı)", "MeasurementCategory": "Volume" },
      { "Name": "Şalgam Suyu (Acısız)", "MeasurementCategory": "Volume" },
      { "Name": "Reyhan Şerbeti", "MeasurementCategory": "Volume" },
      { "Name": "Demirhindi Şerbeti", "MeasurementCategory": "Volume" },
      { "Name": "Boza", "MeasurementCategory": "Volume" },
      { "Name": "Ayran", "MeasurementCategory": "Volume" },
      { "Name": "Kefir", "MeasurementCategory": "Volume" },
      { "Name": "Elma Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Portakal Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Vişne Suyu", "MeasurementCategory": "Volume" },
      { "Name": "Şeftali Nektarı", "MeasurementCategory": "Volume" },
      { "Name": "Kola", "MeasurementCategory": "Volume" },
      { "Name": "Gazoz", "MeasurementCategory": "Volume" },
      { "Name": "Dökme Siyah Çay (Rize)", "MeasurementCategory": "Weight" },
      { "Name": "Poşet Çay", "MeasurementCategory": "Count" },
      { "Name": "Yeşil Çay", "MeasurementCategory": "Weight" },
      { "Name": "Türk Kahvesi", "MeasurementCategory": "Weight" },
      { "Name": "Dibek Kahvesi", "MeasurementCategory": "Weight" },
      { "Name": "Menengiç Kahvesi", "MeasurementCategory": "Weight" },
      { "Name": "Filtre Kahve", "MeasurementCategory": "Weight" },
      { "Name": "Granül Kahve", "MeasurementCategory": "Weight" },
      { "Name": "Espresso", "MeasurementCategory": "Volume" },
      { "Name": "Buzlu Çay (Ice Tea)", "MeasurementCategory": "Volume" },
      { "Name": "Limonata", "MeasurementCategory": "Volume" },
      { "Name": "Gül Suyu (Yemeklik)", "MeasurementCategory": "Volume" }
    ]
  }
]

""";
        public const string ExtraSeedDataJson = """
{
  "Allergies": [
    {
      "Name": "Yer Fıstığı",
      "Description": "Yer fıstığı ve yer fıstığı yağı içeren ürünlere karşı alerji",
      "IconName": "Nut"
    },
    {
      "Name": "Ağaç Yemişleri",
      "Description": "Ceviz, badem, fındık, kaju gibi kuruyemişlere alerji",
      "IconName": "Tree"
    },
    {
      "Name": "Süt ve Süt Ürünleri",
      "Description": "İnek sütü ve laktoz intoleransı/süt alerjisi",
      "IconName": "Milk"
    },
    {
      "Name": "Yumurta",
      "Description": "Yumurta akı veya sarısına karşı alerji",
      "IconName": "Egg"
    },
    {
      "Name": "Glüten",
      "Description": "Çölyak hastalığı veya glüten hassasiyeti",
      "IconName": "Wheat"
    },
    {
      "Name": "Soya",
      "Description": "Soya fasulyesi, soya sosu ve soya bazlı ürünlere alerji",
      "IconName": "Leaf"
    },
    {
      "Name": "Deniz Ürünleri",
      "Description": "Karides, ıstakoz, kalamar gibi kabuklu deniz ürünleri",
      "IconName": "Fish"
    },
    {
      "Name": "Balık",
      "Description": "Somon, levrek gibi yüzgeçli balıklara karşı alerji",
      "IconName": "Fish"
    },
    {
      "Name": "Susam",
      "Description": "Susam ve tahin gibi ürünlere karşı alerji",
      "IconName": "Droplet"
    },
    {
      "Name": "Hardal",
      "Description": "Hardal tohumu ve sosuna alerji",
      "IconName": "AlertTriangle"
    },
    {
      "Name": "Kereviz",
      "Description": "Kereviz ve kök kereviz alerjisi",
      "IconName": "Plant"
    },
    {
      "Name": "Sülfitler",
      "Description": "Şarap, kurutulmuş meyve ve koruyuculardaki sülfitlere hassasiyet",
      "IconName": "Wine"
    }
  ],
  "Dietary": [
    {
      "Name": "Vegan",
      "Description": "Hiçbir hayvansal ürün (et, süt, yumurta, bal) tüketilmez.",
      "IconName": "Leaf"
    },
    {
      "Name": "Vejetaryen",
      "Description": "Et, tavuk, balık tüketilmez ancak süt ürünleri ve yumurta tüketilebilir.",
      "IconName": "Carrot"
    },
    {
      "Name": "Pesketaryen",
      "Description": "Kırmızı ve beyaz et tüketilmez ancak balık ve deniz ürünleri tüketilir.",
      "IconName": "Fish"
    },
    {
      "Name": "Ketojenik",
      "Description": "Çok düşük karbonhidratlı, yüksek yağlı diyet.",
      "IconName": "Dumbbell"
    },
    {
      "Name": "Paleo",
      "Description": "İşlenmiş gıdalardan, tahıllardan ve süt ürünlerinden kaçınan taş devri diyeti.",
      "IconName": "Bone"
    },
    {
      "Name": "Glütensiz",
      "Description": "Glüten içeren buğday, arpa, çavdar tüketilmeyen diyet.",
      "IconName": "WheatOff"
    },
    {
      "Name": "Helal",
      "Description": "İslami kurallara uygun hazırlanan ve helal sertifikalı ürünleri içeren beslenme biçimi.",
      "IconName": "CheckCircle"
    },
    {
      "Name": "Koşer",
      "Description": "Yahudi beslenme kurallarına (Kaşrut) uygun beslenme.",
      "IconName": "Star"
    },
    {
      "Name": "Düşük Karbonhidrat",
      "Description": "Karbonhidrat alımının kısıtlandığı beslenme şekli.",
      "IconName": "Activity"
    },
    {
      "Name": "Akdeniz",
      "Description": "Zeytinyağı, sebze, meyve, balık ve tam tahıl ağırlıklı diyet.",
      "IconName": "Sun"
    },
    {
      "Name": "Aralıklı Oruç (Intermittent Fasting)",
      "Description": "Sadece belirli saat aralıklarında besin tüketimine izin veren diyet türü.",
      "IconName": "Clock"
    }
  ],
  "RecipeCategories": [
    {
      "Id": 1,
      "Name": "Kahvaltılıklar",
      "IconName": "Coffee"
    },
    {
      "Id": 2,
      "Name": "Çorbalar",
      "IconName": "Bowl"
    },
    {
      "Id": 3,
      "Name": "Ana Yemekler",
      "IconName": "Utensils"
    },
    {
      "Id": 4,
      "Name": "Zeytinyağlılar",
      "IconName": "Leaf"
    },
    {
      "Id": 5,
      "Name": "Salatalar",
      "IconName": "Carrot"
    },
    {
      "Id": 6,
      "Name": "Mezeler ve Ara Sıcaklar",
      "IconName": "Wine"
    },
    {
      "Id": 7,
      "Name": "Deniz Ürünleri",
      "IconName": "Fish"
    },
    {
      "Id": 8,
      "Name": "Hamur İşleri",
      "IconName": "Croissant"
    },
    {
      "Id": 9,
      "Name": "Tatlılar",
      "IconName": "Cake"
    },
    {
      "Id": 10,
      "Name": "İçecekler",
      "IconName": "CupSoda"
    },
    {
      "Id": 11,
      "Name": "Atıştırmalıklar",
      "IconName": "Cookie"
    },
    {
      "Id": 12,
      "Name": "Makarna ve Pilavlar",
      "IconName": "Bowl"
    },
    {
      "Id": 13,
      "Name": "Et Yemekleri",
      "IconName": "Beef"
    },
    {
      "Id": 14,
      "Name": "Tavuk ve Kümes Hayvanları",
      "IconName": "Bird"
    },
    {
      "Id": 15,
      "Name": "Vegan ve Vejetaryen",
      "IconName": "LeafyGreen"
    }
  ],
  "Tags": [
    {
      "Id": 1,
      "Name": "Hızlı ve Pratik"
    },
    {
      "Id": 2,
      "Name": "Düşük Kalorili"
    },
    {
      "Id": 3,
      "Name": "Yüksek Proteinli"
    },
    {
      "Id": 4,
      "Name": "Glütensiz"
    },
    {
      "Id": 5,
      "Name": "Şekersiz"
    },
    {
      "Id": 6,
      "Name": "Fırın Yemeği"
    },
    {
      "Id": 7,
      "Name": "Tencere Yemeği"
    },
    {
      "Id": 8,
      "Name": "Sokak Lezzeti"
    },
    {
      "Id": 9,
      "Name": "Anne Yemeği"
    },
    {
      "Id": 10,
      "Name": "Öğrenci İşi"
    },
    {
      "Id": 11,
      "Name": "Bütçe Dostu"
    },
    {
      "Id": 12,
      "Name": "Misafirlik"
    },
    {
      "Id": 13,
      "Name": "Yöresel"
    },
    {
      "Id": 14,
      "Name": "Dünya Mutfağı"
    },
    {
      "Id": 15,
      "Name": "Çocuklar İçin"
    },
    {
      "Id": 16,
      "Name": "Diyet"
    },
    {
      "Id": 17,
      "Name": "Airfryer"
    },
    {
      "Id": 18,
      "Name": "Mangal & Barbekü"
    },
    {
      "Id": 19,
      "Name": "Kış Hazırlığı"
    },
    {
      "Id": 20,
      "Name": "Fermente"
    },
    {
      "Id": 21,
      "Name": "Sıcak Servis"
    },
    {
      "Id": 22,
      "Name": "Soğuk Servis"
    },
    {
      "Id": 23,
      "Name": "Acılı"
    },
    {
      "Id": 24,
      "Name": "Bayram Yemeği"
    },
    {
      "Id": 25,
      "Name": "Glisemik İndeksi Düşük"
    },
    {
      "Id": 26,
      "Name": "Kansızlık Giderici"
    },
    {
      "Id": 27,
      "Name": "Bağışıklık Güçlendirici"
    },
    {
      "Id": 28,
      "Name": "Probiyotik"
    },
    {
      "Id": 29,
      "Name": "Tatil Kaçamağı"
    },
    {
      "Id": 30,
      "Name": "Şefin İmza Tarifi"
    }
  ]
}
""";
        public const string RecipesJson = """
[
  {
    "Title": "Klasik Mercimek Çorbası",
    "Description": "Ev yapımı nefis mercimek çorbası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 30,
    "Ingredients": [
      {
        "Name": "Kırmızı Mercimek",
        "Amount": 200,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Patates",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Trabzon Tereyağı",
        "Amount": 30,
        "Unit": "Gram"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Sebzeleri doğrayın.",
      "Tereyağında hafif kavurun.",
      "Mercimekleri ve suyu ekleyip kaynatın.",
      "Blenderdan geçirin."
    ],
    "Categories": [
      "Çorbalar",
      "Vegan ve Vejetaryen"
    ],
    "Tags": [
      "Anne Yemeği",
      "Sıcak Servis",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Patlıcan Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Patlıcan Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif patlıcan kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Yöresel",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Patlıcan Sote",
    "Description": "Domates ve biberle şenlenmiş patlıcan sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Patlıcan Fırın",
    "Description": "Fırında hafifçe pişmiş patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Yöresel",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Patlıcan Salatası",
    "Description": "Zeytinyağlı limonlu patlıcan salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Patlıcan ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Yöresel",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kabak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kabak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif kabak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kabak Sote",
    "Description": "Domates ve biberle şenlenmiş kabak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kabak Fırın",
    "Description": "Fırında hafifçe pişmiş kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kabak Salatası",
    "Description": "Zeytinyağlı limonlu kabak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Kabak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Ispanak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Ispanak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif ispanak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Bağışıklık Güçlendirici",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Ispanak Sote",
    "Description": "Domates ve biberle şenlenmiş ispanak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Ispanak Fırın",
    "Description": "Fırında hafifçe pişmiş ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Bağışıklık Güçlendirici",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Ispanak Salatası",
    "Description": "Zeytinyağlı limonlu ispanak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Ispanak ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Bağışıklık Güçlendirici",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Pırasa Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Pırasa Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif pırasa kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Kış Hazırlığı",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Pırasa Sote",
    "Description": "Domates ve biberle şenlenmiş pırasa sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Pırasa Fırın",
    "Description": "Fırında hafifçe pişmiş pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Kış Hazırlığı",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Pırasa Salatası",
    "Description": "Zeytinyağlı limonlu pırasa salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Pırasa ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Kış Hazırlığı",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Karnabahar Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Karnabahar Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif karnabahar kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Glütensiz",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Karnabahar Sote",
    "Description": "Domates ve biberle şenlenmiş karnabahar sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Karnabahar Fırın",
    "Description": "Fırında hafifçe pişmiş karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Glütensiz",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Karnabahar Salatası",
    "Description": "Zeytinyağlı limonlu karnabahar salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Karnabahar ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Glütensiz",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Brokoli Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Brokoli Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif brokoli kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Düşük Kalorili",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Brokoli Sote",
    "Description": "Domates ve biberle şenlenmiş brokoli sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Brokoli Fırın",
    "Description": "Fırında hafifçe pişmiş brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Düşük Kalorili",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Brokoli Salatası",
    "Description": "Zeytinyağlı limonlu brokoli salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Brokoli ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Düşük Kalorili",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Taze Fasulye Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis taze fasulye.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Anne Yemeği",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Taze Fasulye Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif taze fasulye kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Anne Yemeği",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Taze Fasulye Sote",
    "Description": "Domates ve biberle şenlenmiş taze fasulye sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Anne Yemeği",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Taze Fasulye Fırın",
    "Description": "Fırında hafifçe pişmiş taze fasulye.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Anne Yemeği",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Taze Fasulye Salatası",
    "Description": "Zeytinyağlı limonlu taze fasulye salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Anne Yemeği",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kereviz Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis kereviz.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kereviz Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif kereviz kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kereviz Sote",
    "Description": "Domates ve biberle şenlenmiş kereviz sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kereviz Fırın",
    "Description": "Fırında hafifçe pişmiş kereviz.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Dana Kuşbaşıli Kereviz Salatası",
    "Description": "Zeytinyağlı limonlu kereviz salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Dana Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Kereviz ve Dana Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Dana Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Yüksek Proteinli",
      "Diyet",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Patlıcan Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Patlıcan Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif patlıcan kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Yöresel",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Patlıcan Sote",
    "Description": "Domates ve biberle şenlenmiş patlıcan sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Patlıcan Fırın",
    "Description": "Fırında hafifçe pişmiş patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Yöresel",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Patlıcan Salatası",
    "Description": "Zeytinyağlı limonlu patlıcan salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Patlıcan ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Yöresel",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kabak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kabak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif kabak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kabak Sote",
    "Description": "Domates ve biberle şenlenmiş kabak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kabak Fırın",
    "Description": "Fırında hafifçe pişmiş kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kabak Salatası",
    "Description": "Zeytinyağlı limonlu kabak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Kabak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Ispanak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Ispanak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif ispanak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Bağışıklık Güçlendirici",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Ispanak Sote",
    "Description": "Domates ve biberle şenlenmiş ispanak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Ispanak Fırın",
    "Description": "Fırında hafifçe pişmiş ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Bağışıklık Güçlendirici",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Ispanak Salatası",
    "Description": "Zeytinyağlı limonlu ispanak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Ispanak ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Bağışıklık Güçlendirici",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Pırasa Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Pırasa Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif pırasa kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Kış Hazırlığı",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Pırasa Sote",
    "Description": "Domates ve biberle şenlenmiş pırasa sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Pırasa Fırın",
    "Description": "Fırında hafifçe pişmiş pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Kış Hazırlığı",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Pırasa Salatası",
    "Description": "Zeytinyağlı limonlu pırasa salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Pırasa ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Kış Hazırlığı",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Karnabahar Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Karnabahar Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif karnabahar kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Glütensiz",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Karnabahar Sote",
    "Description": "Domates ve biberle şenlenmiş karnabahar sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Karnabahar Fırın",
    "Description": "Fırında hafifçe pişmiş karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Glütensiz",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Karnabahar Salatası",
    "Description": "Zeytinyağlı limonlu karnabahar salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Karnabahar ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Glütensiz",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Brokoli Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Brokoli Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif brokoli kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Düşük Kalorili",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Brokoli Sote",
    "Description": "Domates ve biberle şenlenmiş brokoli sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Brokoli Fırın",
    "Description": "Fırında hafifçe pişmiş brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Düşük Kalorili",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Brokoli Salatası",
    "Description": "Zeytinyağlı limonlu brokoli salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Brokoli ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Düşük Kalorili",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Taze Fasulye Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis taze fasulye.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Anne Yemeği",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Taze Fasulye Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif taze fasulye kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Anne Yemeği",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Taze Fasulye Sote",
    "Description": "Domates ve biberle şenlenmiş taze fasulye sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Anne Yemeği",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Taze Fasulye Fırın",
    "Description": "Fırında hafifçe pişmiş taze fasulye.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Anne Yemeği",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Taze Fasulye Salatası",
    "Description": "Zeytinyağlı limonlu taze fasulye salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Taze Fasulye",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Taze Fasulye ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Taze Fasulye ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Anne Yemeği",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kereviz Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis kereviz.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kereviz Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif kereviz kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kereviz Sote",
    "Description": "Domates ve biberle şenlenmiş kereviz sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kereviz Fırın",
    "Description": "Fırında hafifçe pişmiş kereviz.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kereviz ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Ana Yemekler"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Kuzu Kuşbaşıli Kereviz Salatası",
    "Description": "Zeytinyağlı limonlu kereviz salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Kuzu Kuşbaşı",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kereviz",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Kereviz ve Kuzu Kuşbaşı malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Kuzu Kuşbaşı ve Kereviz ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Et Yemekleri",
      "Salatalar"
    ],
    "Tags": [
      "Misafirlik",
      "Diyet",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Patlıcan Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Patlıcan Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif patlıcan kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Yöresel",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Patlıcan Sote",
    "Description": "Domates ve biberle şenlenmiş patlıcan sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Yöresel",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Patlıcan Fırın",
    "Description": "Fırında hafifçe pişmiş patlıcan.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Patlıcan ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Yöresel",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Patlıcan Salatası",
    "Description": "Zeytinyağlı limonlu patlıcan salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kemer Patlıcan",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Patlıcan ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Patlıcan ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Salatalar"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Yöresel",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Kabak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Kabak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif kabak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Diyet",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Kabak Sote",
    "Description": "Domates ve biberle şenlenmiş kabak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Diyet",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Kabak Fırın",
    "Description": "Fırında hafifçe pişmiş kabak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Kabak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Diyet",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Kabak Salatası",
    "Description": "Zeytinyağlı limonlu kabak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Kabak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Kabak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Kabak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Salatalar"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Diyet",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Ispanak Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Ispanak Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif ispanak kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Bağışıklık Güçlendirici",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Ispanak Sote",
    "Description": "Domates ve biberle şenlenmiş ispanak sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Bağışıklık Güçlendirici",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Ispanak Fırın",
    "Description": "Fırında hafifçe pişmiş ispanak.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Ispanak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Bağışıklık Güçlendirici",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Ispanak Salatası",
    "Description": "Zeytinyağlı limonlu ispanak salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Ispanak",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Ispanak ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Ispanak ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Salatalar"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Bağışıklık Güçlendirici",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Pırasa Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Pırasa Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif pırasa kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Kış Hazırlığı",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Pırasa Sote",
    "Description": "Domates ve biberle şenlenmiş pırasa sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Kış Hazırlığı",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Pırasa Fırın",
    "Description": "Fırında hafifçe pişmiş pırasa.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Pırasa ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Kış Hazırlığı",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Pırasa Salatası",
    "Description": "Zeytinyağlı limonlu pırasa salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Pırasa",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Pırasa ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Pırasa ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Salatalar"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Kış Hazırlığı",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Karnabahar Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Karnabahar Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif karnabahar kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Glütensiz",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Karnabahar Sote",
    "Description": "Domates ve biberle şenlenmiş karnabahar sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Glütensiz",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Karnabahar Fırın",
    "Description": "Fırında hafifçe pişmiş karnabahar.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Karnabahar ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Glütensiz",
      "Fırın Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Karnabahar Salatası",
    "Description": "Zeytinyağlı limonlu karnabahar salatası.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 10,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Karnabahar",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Limon",
        "Amount": 1,
        "Unit": "Adet"
      }
    ],
    "Steps": [
      "Karnabahar ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Karnabahar ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Salatalar"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Glütensiz",
      "Soğuk Servis"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Brokoli Yemeği",
    "Description": "Tencerede yavaş yavaş pişen nefis brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 40,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Brokoli Kavurması",
    "Description": "Soğanla lezzetlendirilmiş hafif brokoli kavurması.",
    "Difficulty": "Easy",
    "Servings": 4,
    "PrepTime": 10,
    "CookTime": 20,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Düşük Kalorili",
      "Hızlı ve Pratik"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Brokoli Sote",
    "Description": "Domates ve biberle şenlenmiş brokoli sote.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 15,
    "CookTime": 25,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      },
      {
        "Name": "Ev Tipi Domates Salçası",
        "Amount": 20,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Düşük Kalorili",
      "Tencere Yemeği"
    ]
  },
  {
    "Title": "Tavuk Göğsüli Brokoli Fırın",
    "Description": "Fırında hafifçe pişmiş brokoli.",
    "Difficulty": "Medium",
    "Servings": 4,
    "PrepTime": 20,
    "CookTime": 45,
    "Ingredients": [
      {
        "Name": "Tavuk Göğsü",
        "Amount": 300,
        "Unit": "Gram"
      },
      {
        "Name": "Brokoli",
        "Amount": 500,
        "Unit": "Gram"
      },
      {
        "Name": "Kuru Soğan",
        "Amount": 1,
        "Unit": "Adet"
      },
      {
        "Name": "Sızma Zeytinyağı",
        "Amount": 30,
        "Unit": "Mililitre"
      },
      {
        "Name": "Tuzsuz Tereyağı",
        "Amount": 5,
        "Unit": "Gram"
      }
    ],
    "Steps": [
      "Brokoli ve Tavuk Göğsü malzemelerini doğrayın.",
      "Soğanı zeytinyağında hafif pembeleşene kadar kavurun.",
      "Sırasıyla Tavuk Göğsü ve Brokoli ekleyip karıştırın.",
      "Tarife uygun olarak pişirme işlemini tamamlayın.",
      "Sıcak veya soğuk olarak servis yapın."
    ],
    "Categories": [
      "Tavuk ve Kümes Hayvanları",
      "Ana Yemekler"
    ],
    "Tags": [
      "Hızlı ve Pratik",
      "Düşük Kalorili",
      "Fırın Yemeği"
    ]
  }
]
""";
    }
}
