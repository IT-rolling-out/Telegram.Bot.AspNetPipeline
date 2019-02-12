The approach with a common ActionContext and RouteActionDelegate for controllers and default RouteAction (which you can register via MapRoute in mvc builder) 
simplified the development of middlewares, but forces to describe all the controller-specific logic in RouteActionDelegate to at least somehow separate 
it from the RouteAction standard processing logic. And yet, you can customize processing of controller methods by redefining IControllerMethodPreparer or it's services 
(in services collection), and customize ActionContext creating by overriding IContextPreparer.