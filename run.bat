kubectl create namespace idempotent

kubectl config set-context --current --namespace idempotent

helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx/
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

helm install nginx --namespace idempotent ingress-nginx/ingress-nginx -f nginx-ingress-values.yml --atomic

helm install postgres --namespace idempotent bitnami/postgresql --namespace idempotent -f postgres-values.yml --atomic

kubectl create secret generic secret-order-appsettings --namespace idempotent --from-file=./Otus.Microservice.Order/appsettings.secrets.json

kubectl apply -f manifest.yml
