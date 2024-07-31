Archivo Helpers
{{- define "microappstudio01.hosts.authserver" -}}
{{- print "https://" (.Values.global.hosts.authserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.webgateway" -}}
{{- print "https://" (.Values.global.hosts.webgateway | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.kibana" -}}
{{- print "https://" (.Values.global.hosts.kibana | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.grafana" -}}
{{- print "https://" (.Values.global.hosts.grafana | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.blazor" -}}
{{- print "https://" (.Values.global.hosts.blazor | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.webpublic" -}}
{{- print "https://" (.Values.global.hosts.webpublic | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "microappstudio01.hosts.publicgateway" -}}
{{- print "https://" (.Values.global.hosts.publicgateway | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
