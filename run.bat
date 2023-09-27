kubectl create namespace saga

kubectl config set-context --current --namespace saga

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx/
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install nginx --namespace saga ingress-nginx/ingress-nginx -f nginx-ingress-values.yml --atomic

helm install postgres --namespace saga bitnami/postgresql -f postgres-values.yml --atomic

helm install rabbit bitnami/rabbitmq  -f rabbit-values.yml --atomic

helm install release --namespace saga --set rabbitmq.username=admin,rabbitmq.password=secretpassword,rabbitmq.erlangCookie=secretcookie bitnami/rabbitmq

kubectl create secret generic secret-order-appsettings --namespace saga --from-file=./Otus.Microservice.Order/appsettings.secrets.json
kubectl create secret generic secret-payment-appsettings --namespace saga --from-file=./Otus.Microservice.Payment/appsettings.secrets.json
kubectl create secret generic secret-store-appsettings --namespace saga --from-file=./Otus.Microservice.Store/appsettings.secrets.json
kubectl create secret generic secret-delivery-appsettings --namespace saga --from-file=./Otus.Microservice.Delivery/appsettings.secrets.json

kubectl apply -f manifest.yml
