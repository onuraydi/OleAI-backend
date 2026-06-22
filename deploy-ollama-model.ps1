# Ollama modeli için Docker kurulum scripti

$CONTAINER_NAME = "ollama" # Kendi container adınıza göre değiştirin

echo "Modelfile docker container'ına kopyalanıyor..."
docker cp ./OleAI-Editor.Modelfile ${CONTAINER_NAME}:/tmp/OleAI-Editor.Modelfile

echo "Model derleniyor (Bu işlem birkaç dakika sürebilir)..."
docker exec -it $CONTAINER_NAME ollama create oleai-editor -f /tmp/OleAI-Editor.Modelfile

echo "Kurulum tamamlandı! Artık 'oleai-editor:latest' modelini kullanabilirsiniz."
